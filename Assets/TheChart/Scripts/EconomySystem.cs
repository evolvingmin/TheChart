using System;
using System.Collections;
using System.Collections.Generic;
using ChallengeKit;
using ChallengeKit.Pattern;
using UnityEngine;
using Random = UnityEngine.Random;

public class EconomyParser : IParser
{
    EconomySystem economySystem;

    public Define.Result Init(SystemMono parentSystem)
    {
        economySystem = (EconomySystem)parentSystem;

        return Define.Result.OK;
    }

    public bool ParseCommand(string Command, params object[] Objs)
    {
        //MessageSystem.Instance.BroadcastSystems(null, "SetActive", "CandleDataDisplayer", bOpen);
        switch (Command)
        {
            case "StartTransection":
                economySystem.StartTransection((bool)Objs[0], (float)Objs[1]);
                return true;

        }

        return false;
    }
}

public class TransectionReqData
{
    public enum Type
    {
        Buy,
        Sell,
    }

    public int id;
    public Trader trader;
    public Type type;
    public int price;
    public int count;
    public float reqTime;

    public int TotalPrice {  get { return price * count; } }
}

public class TransectionHistoryData
{
    public int buyID;
    public int sellID;
    public Trader buyer;
    public Trader seller;
    public int price;
    public int count;
    public float completeTime;
}

// 
public class EconomySystem : SystemMono
{

    [SerializeField]
    private float basePercent = 5f;

    // 이 가격을 모멘텀이라는 개념으로 바꾸자.
    // 이 모멘텀에 따라 Trader의 수가 증가하고
    // 그 트레이더의 성향에 따라 거래 빈도가 증가한다
    // 트레이더의 상태가 나타나는데, 사고 싶다와 팔고 싶다가 존재해야 한다
    // 트레이더의 상태는 가변적으로 바뀌면서, 판 사람은 사고 싶어지고, 산 사람은 팔고 싶어 한다.

    [SerializeField]
    private float momentum = 0.0f;

    public float Momentum {  get { return momentum / 50; } }

    [SerializeField]
    private float momentumMin = -500;
    [SerializeField]
    private float momentumMax = 500;

    [SerializeField]
    private int startTraderNum = 10;

    [SerializeField]
    private int maxTraderNum = 50;

    [SerializeField]
    private GameObject traderPrefab;

    private Dictionary<string, Trader> traders;

    private int traderNum = 0;

    public int LastPrice { get; private set; } = 5000;

    private float momentumLimitRatio = 0.1f;

    private float affectPercent;

    private List<TransectionReqData> buyList;
    
    private List<TransectionReqData> sellList;

    private List<TransectionHistoryData> transectionHistory;

    // 1초마다 체크.
    private float checkTransection_Tick = 1.0f;
    private float currentTime = 0.0f;
    private float lastTickTime = 0.0f;
    private bool bTransectionHandle = true;

    private int transectionID = 0;

    private ResourceManager resourceManager;

    private void Awake()
    {
        resourceManager = GetComponent<ResourceManager>();
        traders = new Dictionary<string, Trader>(startTraderNum);
        buyList = new List<TransectionReqData>();
        sellList = new List<TransectionReqData>();
        transectionHistory = new List<TransectionHistoryData>();
    }

    public Define.Result Init()
    {
        Define.Result result = base.Init(new EconomyParser());

        resourceManager.SetPrefab<GameObject>("Trader", "Base", traderPrefab, transform);
        
        // 이분들이 주식의 실 소유주입니다.
        // 물론 스톡옵션같은거 처럼 받은 인간들이라서 팔기에 급급할 것이죠 ^^
        int remainTraders = startTraderNum;
        while (remainTraders > 0)
        {
            var traderObject = resourceManager.GetObject<GameObject>("Trader", "Base");
            Trader trader = traderObject.GetComponent<Trader>();
            trader.startStock = Random.Range(1000, 12000);
            trader.plan = Trader.Plan.Sell;
            string Name = "trader_" + traderNum++;
            trader.Init(this, Name);
            traders.Add(Name, trader);
            remainTraders--;
        }

        return result;
    }

    private void Update()
    {
        float maxDeltaMomentum = momentum != 0 ? momentum * momentumLimitRatio : basePercent;
        momentum = Mathf.Clamp(Random.Range(momentum - maxDeltaMomentum, momentum + maxDeltaMomentum), momentumMin, momentumMax);

        // 유입
        if (momentum > 0.0f)
        {
            if(traders.Count < maxTraderNum)
            {
                var traderObject = resourceManager.GetObject<GameObject>("Trader", "Base");
                Trader trader = traderObject.GetComponent<Trader>();
                string Name = "trader_" + traderNum++;
                trader.Init(this, Name);
                traders.Add(Name, trader);
            }
        }

        // 거래 가능 여부 확인 주기.
        currentTime += Time.deltaTime;

        if (currentTime - lastTickTime < checkTransection_Tick)
            return;

        lastTickTime = currentTime;

        if (buyList.Count == 0 || sellList.Count == 0)
            return;

        if (bTransectionHandle == false)
        {
            Debug.Log("Transection Pause this Update Routine!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            return;
        }
            

        int buyIndex = 0;
        int sellIndex = 0;

        while(buyIndex < buyList.Count && sellIndex < sellList.Count)
        {
            TransectionReqData buyData = buyList[buyIndex];
            TransectionReqData sellData = sellList[sellIndex];

            // 비싸게 산다고 하더라도, 실 거래가는 파는 사람의 가격에 맞춰야 한다.
            if (buyData.price >= sellData.price)
            {
                // 일단 이 시점부터 거래 성립.

                bool bBuyingAll = buyData.count * sellData.price < sellData.TotalPrice;
                bool bPerfectTransection = buyData.count * sellData.price == sellData.TotalPrice;

                int transectionCount = bBuyingAll ? buyData.count : sellData.count;

                TransectionHistoryData historyData = new TransectionHistoryData
                {
                    count = transectionCount,
                    price = sellData.price,
                    buyer = buyData.trader,
                    seller = sellData.trader,
                    buyID = buyData.id,
                    sellID = sellData.id,
                    completeTime = currentTime,
                };

                historyData.buyer.TransectionComplete(historyData);
                historyData.seller.TransectionComplete(historyData);

                if(bPerfectTransection)
                {
                    if (buyIndex >= 0 && buyIndex < buyList.Count)
                    {
                        buyList.RemoveAt(buyIndex);
                    }
                    else
                    {
                        Debug.DebugBreak();
                    }
                    if(sellIndex >= 0 && sellIndex < sellList.Count)
                    {
                        sellList.RemoveAt(sellIndex);
                    }
                    else
                    {
                        Debug.DebugBreak();
                    }
                    
                }
                else
                {
                    if (bBuyingAll)
                    {
                        if(buyIndex >= 0 && buyIndex < buyList.Count)
                        {
                            buyList.RemoveAt(buyIndex);
                        }
                        else
                        {
                            Debug.DebugBreak();
                        }
                        
                        sellList[sellIndex].count -= buyData.count;
                    }
                    else if (bBuyingAll == false && sellIndex < sellList.Count)
                    {
                        if(sellIndex >= 0)
                        {
                            sellList.RemoveAt(sellIndex);
                        }
                        else
                        {
                            Debug.DebugBreak();
                        }
                        
                        buyList[buyIndex].count -= sellData.count;
                    }
                }

                if(historyData.count < 1)
                {
                    Debug.Break();
                }

                LastPrice = historyData.price;
                transectionHistory.Add(historyData);
                Debug.LogFormat("Transection : Buyer({0}), Seller({1}), Price({2}), Count({3})", 
                    historyData.buyer.TraderName, historyData.seller.TraderName, historyData.price, historyData.count);
            }
            else
            {
                buyIndex++;
            }
        }
    }

    public void CancelTransectionsAll(string traderName, Dictionary<int, TransectionReqData> reqList, ref int cash, ref int stock)
    {
        bTransectionHandle = false;

        int initialCash = cash;
        int initialStock = stock;

        foreach (var item in reqList)
        {
            if(item.Value.type == TransectionReqData.Type.Buy)
            {
                cash += item.Value.TotalPrice;
            }
            else
            {
                stock += item.Value.count;
            }
        }

        buyList.RemoveAll((x) => x.trader.TraderName == traderName);
        sellList.RemoveAll((x) => x.trader.TraderName == traderName);

        reqList.Clear();
        bTransectionHandle = true;
        Debug.LogFormat("CancelTransectionAll CashBack({0} -> {1}), StockBack({2} -> {3})", initialCash, cash, initialStock, stock);
    }

    public void OutTrader(Trader trader)
    {
        Debug.Log("trader Out ! : " + trader.TraderName);
        traders.Remove(trader.TraderName);
        trader.gameObject.SetActive(false);
        resourceManager.CollectGameObject("Trader", trader.gameObject);
    }

    void OnGUI()
    {
        if(buyList.Count != 0)
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(10, 300, 600, 20), string.Format("BuyRange Min {0} , Max {1}, Count {2}", 
                buyList[0].price, buyList[buyList.Count - 1].price, buyList.Count));
        }

        if (sellList.Count != 0)
        {
            GUI.color = Color.blue;
            GUI.Label(new Rect(10, 315, 600, 20), string.Format("SellRange Min {0} , Max {1}, Count {2}", 
                sellList[0].price, sellList[sellList.Count - 1].price, sellList.Count));
        }

        GUI.color = Color.black;

        GUI.Label(new Rect(10, 330, 600, 20), string.Format("Last Price :{0}", LastPrice));
        // buyer, seller total
        //GUI.Label(new Rect(10, 0, 600, 80), resourceManager.ToString());
    }

    public void RequestTransection(TransectionReqData transectionData)
    {
        bTransectionHandle = false;
        transectionData.id = transectionID++;

        if (transectionData.type == TransectionReqData.Type.Buy)
        {
            buyList.Add(transectionData);
            buyList.Sort((x, y) => x.price.CompareTo(y.price));
        }
        else
        {
            sellList.Add(transectionData);
            sellList.Sort((x, y) => x.price.CompareTo(y.price));
        }
        bTransectionHandle = true;

    }

    public void StartTransection(bool bUp, float Scale)
    {
        affectPercent = basePercent * ( bUp ? 1 : -1 ) * Scale;
        momentum = (int)(momentum * affectPercent + momentum);
    }
}
