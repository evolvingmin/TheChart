using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChallengeKit;
using ChallengeKit.Pattern;

// 지금은 그냥 터치해서 스와이프하면 무조건 사고 팔게, 나중에는 유저가 인지할 수 있는 인디케이터가 있어서 그거 끌고 내려서 살수 있도록.
public class ChartTradeArrow : UIComponent
{
    [SerializeField]
    private Color upColor;
    [SerializeField]
    private Color downColor;

    private SpriteRenderer arrowRenderer;

    private float baseLengthY;

    private bool bArrowDisapear = false;
    private bool bUp = false;
    private Vector2 startPos;

    [SerializeField]
    private float fadeAwaySizeX;
    [SerializeField]
    private float fadeAwayTime;

    private Coroutine fadeAwayCorutine;

    private Color baseColor;
    private Vector3 baseScale;

    private void Awake()
    {
        arrowRenderer = GetComponent<SpriteRenderer>();

        baseLengthY = arrowRenderer.size.y;
        baseColor = arrowRenderer.color;
        baseScale= transform.localScale;

    }


    public override void HandleSwipe(float startX, float startY, float endX, float endY, float velocityX, float velocityY)
    {
        if (bArrowDisapear)
            return;

        Vector3 startWorld = Camera.main.ScreenToWorldPoint(new Vector2(startX, startY));
        Vector3 endWorld = Camera.main.ScreenToWorldPoint(new Vector2(endX, endY));
        float distance = Vector3.Distance(startWorld, endWorld);
        startWorld.z = endWorld.z = transform.position.x;

        float distanceY = endWorld.y - startWorld.y;

        transform.position = new Vector3(startWorld.x + ( endWorld.x - startWorld.x ) / 2, startWorld.y + ( endWorld.y - startWorld.y ) / 2, startWorld.z);

        transform.localRotation.Rotate2D(startWorld, endWorld);

        transform.localScale = new Vector3(1, distance / baseLengthY,1);

        bUp = velocityY > 0;
        arrowRenderer.color = bUp ? upColor : downColor;

        StartTransection();
    }

    public override void BeginDrag(float screenX, float screenY)
    {
        if (bArrowDisapear)
            return;

        startPos = new Vector2(screenX, screenY);

        //Debug.LogFormat("BeginDrag, screen X : {0}, scrrenY : {1}", screenX, screenY);
    }
    public override void DragTo(float screenX, float screenY)
    {
        if (bArrowDisapear)
            return;

        Vector3 startWorld = Camera.main.ScreenToWorldPoint(startPos);
        Vector3 endWorld = Camera.main.ScreenToWorldPoint(new Vector2(screenX, screenY));
        float distance = Vector3.Distance(startWorld, endWorld);
        startWorld.z = endWorld.z = transform.position.x;

        float distanceY = endWorld.y - startWorld.y;

        transform.position = new Vector3(startWorld.x + ( endWorld.x - startWorld.x ) / 2, startWorld.y + ( endWorld.y - startWorld.y ) / 2, startWorld.z);

       
        transform.localRotation.Rotate2D(startWorld, endWorld);
        transform.localScale = new Vector3(1, distance / baseLengthY, 1);

        bUp = screenY - startPos.y > 0;
        arrowRenderer.color = bUp ? upColor : downColor;
    }
    public override void EndDrag(float velocityXScreen, float velocityYScreen)
    {
        if (bArrowDisapear)
            return;

        bArrowDisapear = true;
        StartTransection();
        //Debug.LogFormat("EndDrag, velocityXScreen: {0}, velocityYScreen : {1}", velocityXScreen, velocityYScreen);
    }

    public void StartTransection()
    {
        MessageSystem.Instance.BroadcastSystems(null, "StartTransection", bUp, transform.localScale.y);

        if (fadeAwayCorutine != null)
        {
            StopCoroutine(fadeAwayCorutine);
            arrowRenderer.color = baseColor;


        }
        
        fadeAwayCorutine = StartCoroutine(FadeAway());
    }

    IEnumerator FadeAway()
    {
        float duration = 0;
        Color beginColor = baseColor;
        Vector3 beginSize = transform.localScale;
        Vector3 endSizeTrans = new Vector3(fadeAwaySizeX, beginSize.y, beginSize.z);

        Color beginColorTrans = new Color(beginColor.r, beginColor.g, beginColor.b, 0);

        while (duration < fadeAwayTime)
        {
            duration += Time.deltaTime;
            arrowRenderer.color = Color.Lerp(beginColor, beginColorTrans, duration / fadeAwayTime);
            transform.localScale = Vector3.Lerp(beginSize, endSizeTrans, duration / fadeAwayTime);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        bArrowDisapear = false;
    }
}
