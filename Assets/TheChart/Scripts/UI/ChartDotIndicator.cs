using ChallengeKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartDotIndicator : UIComponent
{
    [SerializeField]
    private float maxScale = 2.0f;

    [SerializeField]
    private Color completeNofifyColor;

    private float baseZ;

    private float indicatorScaleUpTime;

    [SerializeField]
    private float addtionalAfterImageTime = 0.2f;

    private Vector3 startScale;
    private Coroutine showCorutine;

    private SpriteRenderer dotRenderer;

    private Color baseColor;
    private float showDuration;
    private Vector2 startPos;

    private void Awake()
    {
        baseZ = transform.localPosition.z;
        startScale = transform.localScale;
        showDuration = 0.0f;

        dotRenderer = GetComponent<SpriteRenderer>();
        baseColor = dotRenderer.color;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void HandleSwipe(float startX, float startY, float endX, float endY, float velocityX, float velocityY)
    {
        StopShow();
    }

    public override void PointerDown(float positionX, float positionY, float longTabDuration)
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(positionX, positionY, baseZ));

        indicatorScaleUpTime = longTabDuration;
        startPos = new Vector2(positionX, positionY);
        StartShow();

    }

    public override void PointerUp(float positionX, float positionY)
    {
        if (showDuration < indicatorScaleUpTime)
        {
            StopShow();
        }
    }


    private void StartShow()
    {
        StopShow();
        gameObject.SetActive(true);
        showCorutine = StartCoroutine(Show());
        transform.localScale = startScale;
        dotRenderer.color = baseColor;
    }

    private void StopShow()
    {
        if (showCorutine != null)
        {
            StopCoroutine(showCorutine);
        }
        gameObject.SetActive(false);
    }

    IEnumerator Show()
    {
        showDuration = 0.0f;
        Color currentColor = baseColor;

        float totalDuration = indicatorScaleUpTime + addtionalAfterImageTime;

        // 뭔가 이런거 반복작업 되어 가는거 같은데, 트윅 관련으로 하나 리서치 해야 할듯?
        // 그리고 왜 초반부에 이런식으로 폴리싱에 시간 쓰는거 좋지 않다. 방향성이 없다고 아무거나 잡지말고. 이런 트위킹이 진짜 중요하냐?
        while (showDuration < totalDuration)
        {
            showDuration += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, startScale * maxScale, showDuration / totalDuration);
            dotRenderer.color = Color.Lerp(currentColor, completeNofifyColor, showDuration / totalDuration);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        gameObject.SetActive(false);
    }
}
