using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChallengeKit;
using System;
using TMPro;

// 게임 오브젝트와 긴밀히 연결되는 종류들은 전부 게임 오브젝트와 균일한 위치에 있도록 만들예정.

public class Ruler : MonoBehaviour
{
    [SerializeField]
    private ResourceManager resourceManager;

    [SerializeField]
    private SpriteRenderer boardRendrer;

    [SerializeField]
    private int lineCount = 4; // 최초 시작과 끝을 제외한 4라인 

    [SerializeField]
    private float lineHeight = 0.02f;

    [SerializeField]
    private int unitLinePercent = 5;

    [SerializeField]
    private GameObject linePrefab;

    [SerializeField]
    private GameObject unitLinePrefab;

    [SerializeField]
    private GameObject numberPrefab;

    private List<TextMesh> numbers;


    public Define.Result Init(float leftMargin, float width, float height, float positionLowY, float positionHighY)
    {
        numbers = new List<TextMesh>();
        boardRendrer.size = new Vector2(leftMargin, height);
        boardRendrer.transform.localPosition = new Vector3(-width + leftMargin/2, 0, 0);

        resourceManager.SetPrefab<GameObject>("ruler", "line", linePrefab, transform);
        resourceManager.SetPrefab<GameObject>("ruler", "unitLine", unitLinePrefab, transform);
        resourceManager.SetPrefab<GameObject>("ruler", "number", numberPrefab, transform);


        // line & number
        float lineMargin = ( positionHighY - positionLowY ) / lineCount;
        float baseLinePositionY = positionLowY;

        float baseNumberPositionX = boardRendrer.transform.position.x + leftMargin / 2;
        float baseNumberOffsetY = numberPrefab.GetComponent<Renderer>().bounds.size.y;

        //linePrefab.GetComponent<SpriteRenderer>().size = new Vector2(width, 0.15f);
        for (int i = 0; i <= lineCount; i++)
        {
            var lineObject = resourceManager.GetObject<GameObject>("ruler", "line");
            lineObject.transform.position = new Vector3(width - leftMargin/2, baseLinePositionY, 0);
            lineObject.GetComponent<SpriteRenderer>().size = new Vector2(width * 2 - leftMargin, lineHeight);

            var numberObject = resourceManager.GetObject<GameObject>("ruler", "number");
            
            numberObject.transform.position = new Vector3(baseNumberPositionX, baseNumberOffsetY +  baseLinePositionY, 0);

            numberObject.GetComponent<Renderer>().sortingOrder = 2;
            numbers.Add(numberObject.GetComponent<TextMesh>());

            baseLinePositionY += lineMargin;
        }
        // UI 형식 프리펩은 인스턴스에 예제 용도로 하나 올려두는게 더 좋다.
        linePrefab.SetActive(false);
        numberPrefab.SetActive(false);

        // unitLine
        int unitLineCount = 100 / unitLinePercent;
        float unitlLineMargine = ( positionHighY - positionLowY ) / unitLineCount;
        float baseUintLinePositionY = positionLowY;


        float baseUnitLinePositionX = boardRendrer.transform.position.x + leftMargin / 2 - unitLinePrefab.GetComponent<SpriteRenderer>().size.x / 2;

        for (int i =0; i <= unitLineCount; i++)
        {
            var lineObject = resourceManager.GetObject<GameObject>("ruler", "unitLine");

            lineObject.transform.position = new Vector3(baseUnitLinePositionX, baseUintLinePositionY, 0);

            baseUintLinePositionY += unitlLineMargine;
        }
        unitLinePrefab.SetActive(false);

        return Define.Result.OK;
    }

    public void UpdateNumbers(int lowPrice, int highPrice)
    {
        int unitPrice = ( highPrice - lowPrice ) / lineCount;
        int basePrice = lowPrice;

        foreach (var number in numbers)
        {
            number.text = basePrice.ToString();
            basePrice += unitPrice;
        }
    }
}
