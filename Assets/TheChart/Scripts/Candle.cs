using System;
using System.Collections;
using System.Collections.Generic;
using ChallengeKit;

using UnityEngine;

public class Candle : MonoBehaviour
{
    [Serializable]
    public struct Data
    {
        public  int open;
        public  int end;
        public  int high;
        public  int low;
    }

    public enum State
    {
        None,
        Complete,
        OnProgress
    }


    private Chart chart;
    private Data data;

    private State state = State.None;

    [SerializeField]
    private SpriteRenderer bodyRenderer;

    [SerializeField]
    private SpriteRenderer shadowRenderer;

    public bool IsUp
    {
        get
        {
            return data.end > data.open;
        }
    }

    private void Update()
    {
        if (state == State.None)
            return;

        UpdateVisualization(data);
    }

    public Define.Result Init(Chart chart)
    {
        this.chart = chart;
        transform.localPosition = new Vector3(chart.CandleStartPosRoot.x + bodyRenderer.size.x / 2, chart.CandleStartPosRoot.y, chart.CandleStartPosRoot.z);

        chart.CandleStartPosRoot = new Vector3(chart.CandleStartPosRoot.x + bodyRenderer.size.x, chart.CandleStartPosRoot.y, chart.CandleStartPosRoot.z);
        return Define.Result.OK;
    }

    public void UpdatePriceRealTime(bool bNewCandle, int newPrice)
    {
        if(bNewCandle)
        {
            data.open = newPrice;
            Debug.Log("new Candle!!!!!!!!!!!!!!!! open price!" + data.open);
            data.end = newPrice;
            data.high = newPrice;
            data.low = newPrice;
            state = State.OnProgress;

            UpdateVisualization(data);
        }
        else
        {
            data.end = newPrice;
            data.high = Mathf.Max(data.high, newPrice);

            if(data.high != newPrice)
            {
                Debug.Log("high Price reset!" + data.high);
            }

            data.low = Mathf.Min(data.low, newPrice);

            if (data.low != newPrice)
            {
                Debug.Log("Low Price reset!" + data.low);
            }
        }

    }

    public void UpdateEnd(int lastPrice)
    {
        data.end = lastPrice;

        data.high = Mathf.Max(data.high, lastPrice);
        data.low = Mathf.Min(data.low, lastPrice);

        state = State.Complete;
    }

    private void UpdateVisualization(Data data)
    {
        // 좌우는 절대값으로 움직인다
        // 하지만 위아래 켄들의 크기는 가변 비율로서 잡아야 한다.
        // 캔들에서 어떤 시점을 바라보고 있느냐에 따라서 그 가변비율을 실시간으로 바꿔야 한다.

        // 카메라 기준에서는 위아래 유닛 기준 5, 5 총합 10을 쓴다. 즉 켄들의 가격이 수직상승을 계속 하더라도
        // 해당 범위를 넘어서는 안된다.
        // 다만 모바일이고, 가변 사이즈란 말이야? 즉 사이즈의 최대, 최소 비율을 내가 정한걸로 가져오면 안되고
        // 카메라가 정해주는 비율로 가져와야 깔끔하게 만들 수 있다.

        // shadow, sprite 둘다 스프라이트 렌더러 쓰기로 했다.

        // 또한 가격이 갱신될때마다, 위아래 포지션에 대해서는 만들어진 켄들 모두가 변동되어야 한다.
        // 즉 개별 업데이트가 필요하다. 모든 캔들이 가격이 갱신 될 때마다 업데이트가 불려져야 한다는거다.


        if (state == State.OnProgress)
        {
            if (IsUp)
            {
                bodyRenderer.color = Color.red;
                shadowRenderer.color = Color.red;
            }
            else
            {
                bodyRenderer.color = Color.blue;
                shadowRenderer.color = Color.blue;
            }
        }


        float endPositionY = chart.GetPositionYInChart(data.end);
        float openPositionY = chart.GetPositionYInChart(data.open);
        float lowPositionY = chart.GetPositionYInChart(data.low);
        float highPositionY = chart.GetPositionYInChart(data.high);
        
        // 님.. 몇번이고 이야기 하지만, 수학 계산하려면 무조건 끄적여라. 니는 끄적이지 않으면 수학적인 계산 자체가 발동이 안된다.

        float bodyPosition = (endPositionY - openPositionY) / 2 + openPositionY;
        bodyRenderer.transform.localPosition = new Vector3(0, bodyPosition - transform.localPosition.y, 0);
        bodyRenderer.size = new Vector2(bodyRenderer.size.x, ( endPositionY - openPositionY ));

        float shadowPosition = (highPositionY - lowPositionY) / 2 + lowPositionY;
        shadowRenderer.transform.localPosition = new Vector3(0, shadowPosition - transform.localPosition.y, 0);
        shadowRenderer.size = new Vector2(shadowRenderer.size.x, ( highPositionY - lowPositionY ));
    }
}
