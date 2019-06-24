using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChallengeKit;
using ChallengeKit.Pattern;
using System.Linq;
using System;
using Random = UnityEngine.Random;

[System.Serializable]
public class CandleData
{
    public int open;
    public int end;
    public int high;
    public int low;

    public CandleData(int initPrice)
    {
        open = initPrice;
        end = initPrice;
        high = initPrice;
        low = initPrice;
    }

    public bool Update(int newPrice)
    {
        end = newPrice;
        high = Mathf.Max(high, newPrice);
        low = Mathf.Min(low, newPrice);

        return high != newPrice || low != newPrice;
    }
}

public class Chart : Singleton<Chart>
{
    [SerializeField]
    private EconomySystem economySystem;

    [SerializeField]
    private GameObject candlePrefab;

    // 화면에 표시하는 켄들 수
    private List<Candle> candles;

    // 가격 변동을 보관하기 위해 저장하는 raw 데이터.
    private List<CandleData> candleDatas;

    [SerializeField]
    private Camera chartCamera;

    [SerializeField]
    private Ruler ruler;

    [SerializeField]
    private ResourceManager resourceManager;

    //private int LastPrice {  get { return economySystem.LastPrice; } }

    private int chartPriceLow = 0;

    private int chartPriceHigh = 10000; // 일단 어림짐작으로.

    // 가변 카메라 비율 얻어 올 수 있도록 수정.
    [SerializeField]
    private float leftMargin = 2.0f;

    private float positionLowY = 0.5f;
    private float positionHighY = 9.5f;
    
    [SerializeField]
    private float candleWidth = 0.32f;

    private Vector3 candleStartPosRoot = Vector3.zero;

    private bool isDirty = false;
    public bool IsDirty { get { return isDirty; } }

    // tickControl

    private int dataIndex;

    private float lastTickTime;

    [SerializeField]
    private float unitTime_Tick = 0.5f;

    [SerializeField]
    private float unitTime_Candle = 3.0f;

    private float startTime;
    private float currentTime;
    private float endTime;

    // new 경제 관련 제어
    private int lastPrice = 5000; // 랜덤값 제어로 재 갱신, 일단 트레이더 부분은 좀 더 고민이 필요하다.
    float waitQuery = 0.0f;

    public Vector3 CandleStartPosRoot
    {
        get
        {
            return candleStartPosRoot;
        }

        set
        {
            candleStartPosRoot = value;
        }
    }

    private int maxCandleCount;

    private void Awake()
    {
        candles = new List<Candle>();
        candleDatas = new List<CandleData>();
    }

    public Define.Result Init()
    {
        candlePrefab.GetComponent<Candle>().SetBodySize(candleWidth);
        resourceManager.SetPrefab<GameObject>("Candle", "Base", candlePrefab, transform);

        candleStartPosRoot = new Vector3(0, chartCamera.orthographicSize, 0);

        chartCamera.transform.position = new Vector3(chartCamera.aspect * chartCamera.orthographicSize - leftMargin, chartCamera.orthographicSize, chartCamera.transform.position.z);

        maxCandleCount = (int)( ( ( chartCamera.aspect * chartCamera.orthographicSize - leftMargin / 2 ) * 2 ) / candleWidth );
        Debug.Log("Chart Awake, MaxCandleCount : " + maxCandleCount);

        ruler.Init(leftMargin, chartCamera.aspect * chartCamera.orthographicSize, chartCamera.orthographicSize * 2, positionLowY, positionHighY);
        ruler.UpdateNumbers(chartPriceLow, chartPriceHigh);

        return Define.Result.OK;
    }

    private void Start()
    {
        startTime = 0;
        lastTickTime = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        // 1. 시간축과 현재 시간대를 이용하여, 초기 세팅
        // 1-1 캔들 생성
        // 1-2 캔들 데이터 생성 여부 체크.
        currentTime += Time.deltaTime;

        if (currentTime - lastTickTime < unitTime_Tick)
            return;
        
        lastTickTime = currentTime;

        dataIndex = (int)( currentTime / unitTime_Candle );

        bool bNewCandleData = candleDatas.Count <= dataIndex;

        if (bNewCandleData)
        {
            if (candles.Count < maxCandleCount)
            {
                var CandleObject = resourceManager.GetObject<GameObject>("Candle", "Base");
                Candle candle = CandleObject.GetComponent<Candle>();
                candles.Insert(dataIndex, candle);
                candle.Init(dataIndex, this);
            }
            else
            {
                GetCandleByIndex(dataIndex).Reset(dataIndex);
            }
        }

        int maxDeltaPrice = (int)(lastPrice * 0.015f);
        maxDeltaPrice = Mathf.Max(500, maxDeltaPrice);
        lastPrice = Mathf.Max(Random.Range(lastPrice - maxDeltaPrice, lastPrice + maxDeltaPrice + 50), 0);

        // 2. 갱신된 가격정보로 켄들 데이터 갱신.
        if (bNewCandleData)
        {
            if(candleDatas.Count > 1)
            {
                GetCandleDataByIndex(dataIndex - 1).Update(lastPrice);

                var candleData = GetCandleDataByIndex(dataIndex - 1);
                Debug.LogFormat(" open : {0}, end : {1}, low {2}, high : {3}", candleData.open, candleData.end, candleData.low, candleData.high);
            }

            candleDatas.Add(new CandleData(lastPrice));
            
        }
        else
        {
            GetCandleDataByIndex(dataIndex).Update(lastPrice);
        }

        // 3. 갱신된 데이터 인덱스 기반으로 화면 영역을 넘어간 후처리
        // 3 - 1 최소, 최대 가격 범위 다시 정함.
        // 3 - 2 카메라 이동.
        UpdateChartView(lastPrice, bNewCandleData, dataIndex >= maxCandleCount);

        // 4. 데이터를 기준으로 바로 갱신이 필요한 캔들들은 직접 InvalidateUI 호출.
        // 4 - 1 새 캔들 생성되면, 이전 캔들에겐 마지막 데이터로 종료
        // 4 - 2 새 캔들은 현재 켄들 데이터 기준으로 갱신.
        if (candleDatas.Count > 1 && bNewCandleData)
        {
            GetCandleByIndex(dataIndex - 1).InvalidateUI(false);
        }

        GetCandleByIndex(dataIndex).InvalidateUI();
    }

    private void UpdateChartView(int lastPrice, bool bNewCandleData, bool bNewRange)
    {
        int newHighPrice, newLowPrice;

        if(bNewRange && bNewCandleData)
        {
            chartCamera.transform.position = new Vector3(chartCamera.transform.position.x + candleWidth, chartCamera.transform.position.y, chartCamera.transform.position.z);

            var sortedByHighList = candles.OrderBy(si => si.CandleData.high).ToList();
            newHighPrice = sortedByHighList[sortedByHighList.Count - 1].CandleData.high;
            var sortedByLowList = candles.OrderBy(si => si.CandleData.low).ToList();
            newLowPrice = sortedByLowList[0].CandleData.low;
        }
        else
        {
            newHighPrice = Mathf.Max(lastPrice, chartPriceHigh);
            newLowPrice = Mathf.Min(lastPrice, chartPriceLow);
        }


        isDirty = newHighPrice != chartPriceHigh || newLowPrice != chartPriceLow;

        chartPriceHigh = newHighPrice;
        chartPriceLow = newLowPrice;

        if(chartPriceHigh == chartPriceLow)
        {
            chartPriceLow = 0;
        }

        if (isDirty && ruler)
        {
            ruler.UpdateNumbers(chartPriceLow, chartPriceHigh);
        }
    }

    public float GetPositionYInChart(int Price)
    {
        float priceRatio = ( Price - chartPriceLow ) / (float)( chartPriceHigh - chartPriceLow );
        return Mathf.Clamp(( positionHighY - positionLowY ) * priceRatio + positionLowY, positionLowY, positionHighY);
    }

    public Candle GetCandleByIndex(int dataIndex)
    {
        return candles[dataIndex % maxCandleCount ];
    }

    public CandleData GetCandleDataByIndex(int dataIndex)
    {
        return candleDatas[dataIndex];
    }

    public void AddTransection(float affectPercent)
    {
        waitQuery = affectPercent;
    }

}
