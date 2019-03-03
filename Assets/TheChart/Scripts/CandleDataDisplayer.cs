using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ChallengeKit;

public class CandleDataDisplayer : UIComponent
{
    [SerializeField]
    private TextMeshProUGUI openLabel = null;
    [SerializeField]
    private TextMeshProUGUI endLabel = null;
    [SerializeField]
    private TextMeshProUGUI lowLabel = null;
    [SerializeField]
    private TextMeshProUGUI highLabel = null;
    [SerializeField]
    private TextMeshProUGUI openPrice = null;
    [SerializeField]
    private TextMeshProUGUI endPrice = null;
    [SerializeField]
    private TextMeshProUGUI lowPrice = null;
    [SerializeField]
    private TextMeshProUGUI highPrice = null;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        transform.position = Input.mousePosition;
    }

    private void ResetData(CandleData candleData)
    {
        openPrice.text = candleData.open.ToString();
        endPrice.text = candleData.end.ToString();
        lowPrice.text = candleData.low.ToString();
        highPrice.text = candleData.high.ToString();
    }

    public override void InvalidateUI(params object[] inputs)
    {
        gameObject.SetActive(true);
        ResetData(Chart.Instance.GetCandleDataByIndex((int)inputs[1]));
    }
}
