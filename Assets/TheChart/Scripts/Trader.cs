using ChallengeKit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 이 차트에 참여한 거래자, 플레이어도 포함.
// 스크립터블 오브젝트? 그건 아닌 듯 하다. 
// 물론 트레이더의 성질을 제어하는건, 스크립터블 오브젝트로 뺼 수 있겠다.

// 늘 중요하지만 항상 단순한 버전을 만들어 넣고, 그 이후에 발전시켜야 한다. 안그럼 진도가 안나간다.

public class Trader : MonoBehaviour
{
    public enum Strategy
    {
        Default
    }

    public enum Plan
    {
        Observer,
        Buy,
        Sell
    }

    private EconomySystem economySystem;

    [SerializeField]
    private int startCash; // 초기 자본

    private int currentCash;

    // 초기 참여자에게 강제로 주식 몇정을 주겠다.
    public int startStock;

    private int currentStock;

    [SerializeField]
    private float transectionTime_Tick = 5.0f; // 거래회수. 이 transectionTime_Tick 에 따라 거래자는 반드시 거래를 해야 한다.

    public Plan plan = Plan.Buy;
    private Strategy strategy = Strategy.Default;
    private Dictionary<int, TransectionReqData> reqList;

    // 타이머 관련 기능으로 따로 빼실?
    private float currentTime = 0.0f;
    private float lastTickTime = 0.0f;

    // 트레이더의 성격을 정의하는 핵심적인 지수, 0이면 빅피쳐가 전혀 없다는 거고, 2는 도화지 찢어진다는 의미다.
    // 1은 현가에 즉시 거래, 2는 현재 올라온 가격의 두배 싼 혹은 두배 비싼 가격으로 부른다.
    [Range(0.0f,2.0f)]
    private float bigPictureIndex = 0.0f;

    // 현재 트레이더가 소지한 금액 대비 한번에 얼마나 투자할 것인지를 표현
    private float investRatioIndex = 0.5f;
    // 현재 트레이더가 소지한 주식 수량 대비 얼마나 수익 실현을 할 것인지를 표현
    private float exitRatioIndex = 0.1f;

    private bool bInit = false;

    public string TraderName {  get { return traderName; } }
    private string traderName = "";

    [SerializeField]
    private float TransectionReqLifeTime = 10.0f;

    private float lastTransecionCompleteTime;

    private void Awake()
    {
        reqList = new Dictionary<int, TransectionReqData>();
        lastTickTime = currentTime;
    }

    public Define.Result Init(EconomySystem economySystem, string name)
    {
        this.economySystem = economySystem;

        startCash = UnityEngine.Random.Range(1000000, 10000000);
        currentCash = startCash;
        bigPictureIndex = UnityEngine.Random.Range(0.1f, 10.0f);
        traderName = name;

        currentStock = startStock;

        bInit = true;
        return Define.Result.OK;
    }

    private void Update()
    {
        if (bInit == false)
            return;

        currentTime += Time.deltaTime;

        if (currentTime - lastTransecionCompleteTime > TransectionReqLifeTime && reqList.Count > 0)
        {
            economySystem.CancelTransectionsAll(traderName, reqList, ref currentCash, ref currentStock);
            lastTransecionCompleteTime = currentTime;
        }

        if (currentTime - lastTickTime < transectionTime_Tick)
            return;

        lastTickTime = currentTime;

        TransectionByPlan();

    }

    public void TransectionByPlan()
    {
        // 규칙 바꾸는 것도 정해야 한다.
        plan = UnityEngine.Random.Range(0, 2)  == 1 ? Plan.Sell : Plan.Buy;

        if (CanTransection(plan) == false)
            return;

        switch (strategy)
        {
            case Strategy.Default:
                if(plan == Plan.Buy)
                {
                    TransectionReqData transectionData = new TransectionReqData();
                    transectionData.type = TransectionReqData.Type.Buy;
                    
                    int buyPriceWithBigPicture = (int)(economySystem.LastPrice / bigPictureIndex);
                    transectionData.price = (int)(buyPriceWithBigPicture * ( 1 + economySystem.Momentum ));
                    transectionData.count = (int)(( currentCash * investRatioIndex ) / transectionData.price);
                    transectionData.trader = this;
                    transectionData.reqTime = currentTime;

                    currentCash -= transectionData.TotalPrice;
                    economySystem.RequestTransection(transectionData);
                    reqList.Add(transectionData.id, transectionData);
                }
                else
                {
                    TransectionReqData transectionData = new TransectionReqData();
                    transectionData.type = TransectionReqData.Type.Sell;

                    int sellPriceWithBigPicture = (int)(economySystem.LastPrice * bigPictureIndex);
                    transectionData.price = (int)(sellPriceWithBigPicture * ( 1 + economySystem.Momentum ) );

                    transectionData.count = (int)( ( currentStock * exitRatioIndex ) );
                    transectionData.trader = this;
                    transectionData.reqTime = currentTime;

                    currentStock -= transectionData.count;
                    economySystem.RequestTransection(transectionData);
                    reqList.Add(transectionData.id, transectionData);
                }
                
                break;
        }
    }

    public bool CanTransection(Plan plan)
    {
        int price = (int)( economySystem.LastPrice / bigPictureIndex );
        price = (int)( price * ( 1 + economySystem.Momentum ) );
        int buyableCount = (int)( ( currentCash * investRatioIndex ) / price );
        int SellableStockCount = (int)( ( currentStock * exitRatioIndex ) );

        if (plan == Plan.Buy)
        {
            if(buyableCount <= 0)
            {
                if(reqList.Count > 0)
                {
                    economySystem.CancelTransectionsAll(traderName, reqList, ref currentCash, ref currentStock);
                }
                else
                {
                    if(SellableStockCount < 0)
                    {
                        economySystem.OutTrader(this);
                    }
                }
            }
            
            return buyableCount > 0;
        }
        else
        {
            
            return SellableStockCount > 0;
        }
    }

    public void TransectionComplete(TransectionHistoryData historyData)
    {
        if (historyData.buyer == this)
        {
            currentStock += historyData.count;
            reqList.Remove(historyData.buyID);
        }
        else if(historyData.seller == this)
        {
            currentCash += historyData.price * historyData.count;
            reqList.Remove(historyData.sellID);
        }

        lastTransecionCompleteTime = currentTime;
    }
}

