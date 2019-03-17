using System;
using System.Collections;
using System.Collections.Generic;
using ChallengeKit.Pattern;
using ChallengeKit;

using UnityEngine;


public class Candle : MonoBehaviour
{
    public enum State
    {
        None,
        Complete,
        OnProgress
    }

    private Chart chart;
    private int dataIndex;

    private State state = State.None;

    [SerializeField]
    private SpriteRenderer bodyRenderer;

    [SerializeField]
    private BoxCollider2D bodyCollider;

    [SerializeField]
    private SpriteRenderer shadowRenderer;

    private const float minHeight = 0.02f;

    public bool IsUp
    {
        get
        {
            return CandleData.end > CandleData.open;
        }
    }

    public CandleData CandleData
    {
        get
        {
            return chart.GetCandleDataByIndex(dataIndex);
        }
    }

    private void Update()
    {
        if (state == State.None)
            return;

        UpdateVisualization(CandleData);
    }

    public Define.Result Init(int dataIndex, Chart chart)
    {
        this.chart = chart;

        return Reset(dataIndex);
    }

    public Define.Result Reset(int dataIndex)
    {
        this.dataIndex = dataIndex;

        transform.localPosition = new Vector3(chart.CandleStartPosRoot.x + bodyRenderer.size.x / 2, chart.CandleStartPosRoot.y, chart.CandleStartPosRoot.z);

        chart.CandleStartPosRoot = new Vector3(chart.CandleStartPosRoot.x + bodyRenderer.size.x, chart.CandleStartPosRoot.y, chart.CandleStartPosRoot.z);

        state = State.None;
        return Define.Result.OK;
    }

    public void InvalidateUI(bool bUpdate = true)
    {
        if(bUpdate)
        {
            //Debug.Log("InvalidateUI Chart, Data index is " + dataIndex);
            state = State.OnProgress;
        }

        UpdateVisualization(CandleData);

        if (bUpdate == false)
        {
            state = State.Complete;
        }
    }

    private void UpdateVisualization(CandleData data)
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

        if (state == State.Complete && chart.IsDirty == false)
            return;

        float endPositionY = chart.GetPositionYInChart(data.end);
        float openPositionY = chart.GetPositionYInChart(data.open);
        float lowPositionY = chart.GetPositionYInChart(data.low);
        float highPositionY = chart.GetPositionYInChart(data.high);

        float candleHeight = endPositionY - openPositionY;

        if (Mathf.Abs(candleHeight) < minHeight)
        {
            candleHeight = IsUp ? minHeight : -1 * minHeight;
        }
        
        float bodyPosition = candleHeight / 2 + openPositionY;
        bodyRenderer.transform.localPosition = new Vector3(0, bodyPosition - transform.localPosition.y, 0);
        bodyCollider.offset = new Vector2(0, bodyPosition - transform.localPosition.y);
        SetBodySize(bodyRenderer.size.x, Mathf.Abs(candleHeight));

        float shadowPosition = (highPositionY - lowPositionY) / 2 + lowPositionY;
        shadowRenderer.transform.localPosition = new Vector3(0, shadowPosition - transform.localPosition.y, 0);
        shadowRenderer.size = new Vector2(shadowRenderer.size.x, ( highPositionY - lowPositionY ));
    }

    public void SetBodySize(float candleWidth, float candleHeight = minHeight)
    {
        bodyRenderer.size = new Vector2(candleWidth, candleHeight);
        bodyCollider.size = new Vector2(candleWidth, candleHeight);
    }

    void OnMouseOver()
    {
        //MessageSystem.Instance.BroadcastSystems(null, "InvalidateUI", "CandleDataDisplayer", dataIndex);
    }

    void OnMouseExit()
    {
        //MessageSystem.Instance.BroadcastSystems(null, "SetActive", "CandleDataDisplayer", false);
    }
    

}
