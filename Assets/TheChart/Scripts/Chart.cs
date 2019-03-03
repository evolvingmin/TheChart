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

    public void Update(int newPrice)
    {
        end = newPrice;
        high = Mathf.Max(high, newPrice);
        low = Mathf.Min(low, newPrice);
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

    private int startIndexOnCamera = 0;
    private int lastIndexOnCamera;

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

    public int MaxCandleCount
    {
        get
        {
            return lastIndexOnCamera - startIndexOnCamera + 5;  //여기서 5는 버퍼, 나중에 스크롤 베이스로 이리저리 움직일때 구현해 봐야한다
        }
    }

    private void Awake()
    {
        candles = new List<Candle>();
        candleDatas = new List<CandleData>();

        resourceManager = GetComponent<ResourceManager>();
        resourceManager.Initialize();
        resourceManager.SetPrefab<GameObject>("Candle", "Base", candlePrefab);

        candleStartPosRoot = new Vector3(0, chartCamera.orthographicSize, 0);

        chartCamera.transform.position = new Vector3(chartCamera.aspect * chartCamera.orthographicSize, chartCamera.orthographicSize, chartCamera.transform.position.z);

        startIndexOnCamera = 0;
        lastIndexOnCamera = (int)((chartCamera.aspect * chartCamera.orthographicSize) / 0.32f) * 2; // 켄들사이즈까지 쟤야한다

        Debug.Log("Chart Awake, MaxCandleCount : " + MaxCandleCount);
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
            if (candles.Count < MaxCandleCount)
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

        // 2. 가격 작성
        int maxDeltaPrice = (int)( lastPrice * priceChangeLimitPercent );
        maxDeltaPrice = Mathf.Max(500, maxDeltaPrice);

        lastPrice = Mathf.Max(Random.Range(lastPrice - maxDeltaPrice, lastPrice + maxDeltaPrice), 0);

        chartPriceLow = Mathf.Min(lastPrice, chartPriceLow);
        chartPriceHigh = Mathf.Max(lastPrice, chartPriceHigh);

        // 3. 작성된 가격으로 데이터 갱신.
        if (bNewCandleData)
        {
            if(candleDatas.Count > 1)
            {
                GetCandleDataByIndex(dataIndex - 1).Update(lastPrice);
                GetCandleByIndex(dataIndex - 1).InvalidateUI(false);
            }

            var newCandleData = new CandleData
            {
                open = lastPrice,
                end = lastPrice,
                high = lastPrice,
                low = lastPrice,
            };
            candleDatas.Add(newCandleData);
            Debug.Log("New Candle Generated, Index :" + dataIndex + ", open price is :" + lastPrice);
        }
        else
        {
            GetCandleDataByIndex(dataIndex).Update(lastPrice);
        }

        GetCandleByIndex(dataIndex).InvalidateUI();


        // 4. 갱신된 데이터 인덱스 기반으로 화면 영역을 넘어간 후처리
        // 4 - 1 최소, 최대 가격 범위 다시 정하고
        // 4 - 2 todo : startIndexOnCamera 이전 켄들 비 활성화 및 수집처리. 

        if (dataIndex >= lastIndexOnCamera)
        {
            lastIndexOnCamera++;
            startIndexOnCamera++;

            UpdateChartView();
        }
    }

    private void UpdateChartView()
    {
        chartCamera.transform.position = new Vector3(chartCamera.transform.position.x + 0.32f, chartCamera.transform.position.y, chartCamera.transform.position.z);

        var sortedByHighList = candles.OrderBy(si => si.CandleData.high).ToList();
        chartPriceHigh = sortedByHighList[sortedByHighList.Count - 1].CandleData.high;

        var sortedByLowList = candles.OrderBy(si => si.CandleData.low).ToList();
        chartPriceLow = sortedByLowList[0].CandleData.low;
    }

    public float GetPositionYInChart(int Price)
    {
        float priceRatio = ( Price - chartPriceLow ) / (float)( chartPriceHigh - chartPriceLow );
        return Mathf.Clamp(( positionHighY - positionLowY ) * priceRatio + positionLowY, positionLowY, positionHighY);
    }

    public Candle GetCandleByIndex(int dataIndex)
    {
        return candles[dataIndex % MaxCandleCount ];
    }

    public CandleData GetCandleDataByIndex(int dataIndex)
    {
        return candleDatas[dataIndex];
    }
}
