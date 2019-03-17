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

    //private Color baseColor;
    private Vector3 baseScale;

    private void Awake()
    {
        arrowRenderer = GetComponent<SpriteRenderer>();

        baseLengthY = arrowRenderer.sprite.rect.height / arrowRenderer.sprite.pixelsPerUnit;
        //baseColor = arrowRenderer.color;
        baseScale= transform.localScale;

    }


    public override void HandleSwipe(float startX, float startY, float endX, float endY, float velocityX, float velocityY)
    {
        return;
        /*
        Vector3 startWorld = Camera.main.ScreenToWorldPoint(new Vector2(startX, startY));
        Vector3 endWorld = Camera.main.ScreenToWorldPoint(new Vector2(endX, endY));
        float distance = Vector3.Distance(startWorld, endWorld);
        startWorld.z = endWorld.z = transform.position.x;

        float distanceY = endWorld.y - startWorld.y;

        transform.position = new Vector3(startWorld.x + ( endWorld.x - startWorld.x ) / 2, startWorld.y + ( endWorld.y - startWorld.y ) / 2, startWorld.z);

        transform.Rotate2D(startWorld, endWorld);

        transform.localScale = new Vector3(1, distance / baseLengthY,1);

        bUp = velocityY > 0;
        arrowRenderer.color = bUp ? upColor : downColor;

        StartFadeAway();
        */
    }

    public override void BeginDrag(float screenX, float screenY)
    {
        StopFadeAway();

        startPos = new Vector2(screenX, screenY);

        //Debug.LogFormat("BeginDrag, screen X : {0}, scrrenY : {1}", screenX, screenY);
    }
    public override void DragTo(float screenX, float screenY)
    {
        StopFadeAway();
        

        Vector3 startWorld = Camera.main.ScreenToWorldPoint(startPos);
        Vector3 endWorld = Camera.main.ScreenToWorldPoint(new Vector2(screenX, screenY));
        float distance = Vector3.Distance(startWorld, endWorld);
        startWorld.z = endWorld.z = transform.position.x;

        float distanceY = endWorld.y - startWorld.y;

        if (distanceY == 0.0f)
            return;

        gameObject.SetActive(true);

        transform.position = new Vector3(startWorld.x + ( endWorld.x - startWorld.x ) / 2, startWorld.y + ( endWorld.y - startWorld.y ) / 2, startWorld.z);

        transform.Rotate2D(startWorld, endWorld);

        float scaleFactor = distance / baseLengthY;

        transform.localScale = new Vector3(scaleFactor/2, scaleFactor, 1);

        //https://answers.unity.com/questions/1042119/getting-a-sprites-size-in-pixels.html



        bUp = screenY - startPos.y > 0;
        arrowRenderer.color = bUp ? upColor : downColor;
    }
    public override void EndDrag(float velocityXScreen, float velocityYScreen)
    {
        StartFadeAway();
        //Debug.LogFormat("EndDrag, velocityXScreen: {0}, velocityYScreen : {1}", velocityXScreen, velocityYScreen);
    }

    public void StartFadeAway()
    {
        MessageSystem.Instance.BroadcastSystems(null, "StartTransection", bUp, transform.localScale.y);

        StopFadeAway();
        bArrowDisapear = true;
        gameObject.SetActive(true);
        fadeAwayCorutine = StartCoroutine(FadeAway());
    }

    public void StopFadeAway()
    {
        if (bArrowDisapear == false)
            return;

        gameObject.SetActive(false);

        if (fadeAwayCorutine != null)
        {
            StopCoroutine(fadeAwayCorutine);
        }

        bArrowDisapear = false;
    }

    IEnumerator FadeAway()
    {
        float duration = 0;
        Color beginColor = arrowRenderer.color;
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
