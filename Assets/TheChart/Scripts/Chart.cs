using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChallengeKit;
using System.Linq;

public class Chart : MonoBehaviour
{
    [SerializeField]
    private GameObject candlePrefab;

    private List<Candle> candles;


    [SerializeField]
    private Camera chartCamera;

    private ResourceManager resourceManager;

    // 초단위
    [SerializeField]
    private float timeStandard = 1.0f;

    private float startTime;
    private float currentTime;
    private float endTime;

    // 바로 바로 켄들이 잘만들어지는지 구경해야 하니깐.
    // 그럼 테스트 셈플이 되는 데이터 예시는 델타 세컨드로 존나 때려 박아야 한다.
    // 자꾸 뭘 하는데 거창한 이유를 요구하면 문제가 있는거다.

    // 피곤할때 유트브나 게임으로 자꾸 연장시키면 만성화 된다. 그때 잠시 누워서 자면 훨씬 생산성에 좋다.


    // 테스트 코드 만들때 정교하게 데이터 가정하고 있는게 힘이 든다면, 이런식으로 바로쓰고 나중에 주어 담는
    // 형식으로 정리하자.
    // 그리고 프로토 타이핑 중이다. 다시쓸거 생각하면서 짜는거다.

    private int lastPrice = 5000;

    private int chartPriceLow = 0;
    private int chartPriceHigh = 10000; // 일단 어림짐작으로.

    // 가변 카메라 비율 얻어 올 수 있도록 수정.
    private int positionLowY = 0;
    private int positionHighY = 10;

    private float priceChangeLimitPercent = 0.1f;

    private Vector3 candleStartPosRoot = Vector3.zero;

    private int startIndexOnCamera = 0;
    private int lastIndexOnCamera;

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

    public int ChartPriceLow
    {
        get
        {
            return chartPriceLow;
        }
    }

    public int ChartPriceHigh
    {
        get
        {
            return chartPriceHigh;
        }
    }

    private void Awake()
    {
        candles = new List<Candle>();

        resourceManager = GetComponent<ResourceManager>();
        resourceManager.Initialize();
        resourceManager.SetPrefab<GameObject>("Candle", "Base", candlePrefab);

        candleStartPosRoot = new Vector3(0, chartCamera.orthographicSize, 0);

        chartCamera.transform.position = new Vector3(chartCamera.aspect * chartCamera.orthographicSize, chartCamera.orthographicSize, chartCamera.transform.position.z);

        startIndexOnCamera = 0;
        lastIndexOnCamera = (int)((chartCamera.aspect * chartCamera.orthographicSize) / 0.32f) * 2; // 켄들사이즈까지 쟤야한다
    }

    private void Start()
    {
        startTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = Time.realtimeSinceStartup - startTime;
        int reqIndex = (int)(currentTime / timeStandard);
        bool bNewCandle = false;

        if(candles.Count <= reqIndex)
        {
            // 이부분 비효율적이다.
            var CandleObject = resourceManager.GetObject<GameObject>("Candle", "Base");
            candles.Insert(reqIndex, CandleObject.GetComponent<Candle>());

            candles[reqIndex].Init(this);

            bNewCandle = true;
        }

        // 이걸 이제 경제 시스템인가 뭔가가 계산해야 함.
        // 하지만 내가 차트를 만들 수 있는지를 검증해야 하니깐.
        int maxDeltaPrice = (int)(lastPrice * priceChangeLimitPercent);
        maxDeltaPrice = Mathf.Max(500, maxDeltaPrice);

        lastPrice = Mathf.Max(Random.Range(lastPrice - maxDeltaPrice, lastPrice + maxDeltaPrice) , 0);

        //Debug.Log(lastPrice);

        chartPriceLow = Mathf.Min(lastPrice, chartPriceLow);
        chartPriceHigh = Mathf.Max(lastPrice, chartPriceHigh);

        if(bNewCandle && candles.Count > 1 )
        {
            candles[reqIndex - 1].UpdateEnd(lastPrice);
        }

        candles[reqIndex].UpdatePriceRealTime(bNewCandle, lastPrice);

        if(reqIndex >= lastIndexOnCamera)
        {
            chartCamera.transform.position = new Vector3(chartCamera.transform.position.x + 0.32f, chartCamera.transform.position.y, chartCamera.transform.position.z);

            lastIndexOnCamera++;
            startIndexOnCamera++;

            
            var sortedByHighList = candles.GetRange(startIndexOnCamera, candles.Count - startIndexOnCamera).OrderBy(si => si.CandleData.high).ToList();

            chartPriceHigh = sortedByHighList[sortedByHighList.Count - 1].CandleData.high;

            var sortedByLowList = candles.GetRange(startIndexOnCamera, candles.Count - startIndexOnCamera).OrderBy(si => si.CandleData.low).ToList();

            chartPriceLow = sortedByHighList[0].CandleData.low;
        }

    }

    public float GetPositionYInChart(int Price)
    {
        float priceRatio = ( Price - chartPriceLow ) / (float)( chartPriceHigh - chartPriceLow );
        return Mathf.Clamp(( positionHighY - positionLowY ) * priceRatio + positionLowY, positionLowY, positionHighY);
    }
}
