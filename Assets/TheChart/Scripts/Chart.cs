using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChallengeKit;
using System.Linq;


[System.Serializable]
public class CandleData
{
    public int open;
    public int end;
    public int high;
    public int low;


    public CandleData(int lastPrice)
    {
        open = lastPrice;
        end = lastPrice;
        high = lastPrice;
        low = lastPrice;
    }

    public bool Update(int newPrice)
    {
        end = newPrice;
        high = Mathf.Max(high, newPrice);
        low = Mathf.Min(low, newPrice);

        return high != newPrice || low != newPrice;
    }
}


public class Chart : MonoBehaviour
{
    [SerializeField]
    private GameObject candlePrefab;

    // 화면에 표시하는 켄들 수
    private List<Candle> candles;

    // 가격 변동을 보관하기 위해 저장하는 raw 데이터.
    private List<CandleData> candleDatas;

    [SerializeField]
    private Camera chartCamera;

    private ResourceManager resourceManager;

    private int lastPrice = 5000;

    private int chartPriceLow = 0;
    private int chartPriceHigh = 10000; // 일단 어림짐작으로.

    private float priceChangeLimitPercent = 0.1f;

    // 가변 카메라 비율 얻어 올 수 있도록 수정.
    private int positionLowY = 0;
    private int positionHighY = 10;

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

        resourceManager = GetComponent<ResourceManager>();
        resourceManager.Initialize();
        resourceManager.SetPrefab<GameObject>("Candle", "Base", candlePrefab);

        candleStartPosRoot = new Vector3(0, chartCamera.orthographicSize, 0);

        chartCamera.transform.position = new Vector3(chartCamera.aspect * chartCamera.orthographicSize, chartCamera.orthographicSize, chartCamera.transform.position.z);

        maxCandleCount = (int)( ( chartCamera.aspect * chartCamera.orthographicSize ) / 0.32f ) * 2; ;
        Debug.Log("Chart Awake, MaxCandleCount : " + maxCandleCount);
    }

    private void Start()
    {
        startTime = Time.realtimeSinceStartup;
        lastTickTime = Time.realtimeSinceStartup - startTime;
    }

    // Update is called once per frame
    void Update()
    {
        // 1. 시간축과 현재 시간대를 이용하여, 초기 세팅
        // 1-1 캔들 생성
        // 1-2 캔들 데이터 생성 여부 체크.

        currentTime = Time.realtimeSinceStartup - startTime;

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

        // 2. 가격 갱신.

        int maxDeltaPrice = (int)( lastPrice * priceChangeLimitPercent );
        maxDeltaPrice = Mathf.Max(500, maxDeltaPrice);
        lastPrice = Mathf.Max(Random.Range(lastPrice - maxDeltaPrice, lastPrice + maxDeltaPrice), 0);

        // 3. 작성된 가격으로 데이터 갱신.
        if (bNewCandleData)
        {
            if(candleDatas.Count > 1)
            {
                GetCandleDataByIndex(dataIndex - 1).Update(lastPrice);
            }

            candleDatas.Add(new CandleData(lastPrice));
        }
        else
        {
            GetCandleDataByIndex(dataIndex).Update(lastPrice);
        }

        // 4. 갱신된 데이터 인덱스 기반으로 화면 영역을 넘어간 후처리
        // 4 - 1 최소, 최대 가격 범위 다시 정함.
        // 4 - 2 카메라 이동.

        UpdateChartView(lastPrice, bNewCandleData, dataIndex >= maxCandleCount);


        // 5. 데이터를 기준으로 바로 갱신이 필요한 캔들들은 직접 InvalidateUI 호출.
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
            chartCamera.transform.position = new Vector3(chartCamera.transform.position.x + 0.32f, chartCamera.transform.position.y, chartCamera.transform.position.z);

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
}
