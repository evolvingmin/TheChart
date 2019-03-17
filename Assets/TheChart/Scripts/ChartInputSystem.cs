
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRubyShared;
using ChallengeKit.Pattern;


public class ChartInputSystem : SystemMono
{
    private TapGestureRecognizer tapGesture;
    private LongPressGestureRecognizer      longPressGesture;
    private SwipeGestureRecognizer          swipeGesture;

    [SerializeField]
    private float longPressureDuration = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        CreateTapGesture();
        CreateSwipeGesture();
        CreateLongPressGesture();
    }

    // Update is called once per frame
    void Update()
    {
        CheckOnPointerDown();
        CheckOnPointerUp();
    }

    private void CreateSwipeGesture()
    {
        swipeGesture = new SwipeGestureRecognizer
        {
            Direction = SwipeGestureRecognizerDirection.Any
        };
        swipeGesture.StateUpdated += SwipeGestureCallback;
        swipeGesture.DirectionThreshold = 1.0f; // allow a swipe, regardless of slope
        FingersScript.Instance.AddGesture(swipeGesture);
    }
    

    private void CreateTapGesture()
    {
        tapGesture = new TapGestureRecognizer();
        tapGesture.StateUpdated += TapGestureCallback;
        //tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
        FingersScript.Instance.AddGesture(tapGesture);
    }

    private void CreateLongPressGesture()
    {
        longPressGesture = new LongPressGestureRecognizer
        {
            MaximumNumberOfTouchesToTrack = 1,
            MinimumDurationSeconds = longPressureDuration
        };
        longPressGesture.StateUpdated += LongPressGestureCallback;
        FingersScript.Instance.AddGesture(longPressGesture);
    }

    private void TapGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            MessageSystem.Instance.BroadcastSystems(this, "Tab", gesture.StartFocusX, gesture.StartFocusY);
        }
    }

    private void SwipeGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            MessageSystem.Instance.BroadcastSystems(this, "HandleSwipe", gesture.StartFocusX, gesture.StartFocusY, gesture.FocusX, gesture.FocusY, gesture.VelocityX, gesture.VelocityY);
        }
    }

    private void LongPressGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Began)
        {
            MessageSystem.Instance.BroadcastSystems(this, "BeginDrag", gesture.FocusX, gesture.FocusY);
        }
        else if (gesture.State == GestureRecognizerState.Executing)
        {
            MessageSystem.Instance.BroadcastSystems(this, "DragTo", gesture.FocusX, gesture.FocusY);
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            MessageSystem.Instance.BroadcastSystems(this, "EndDrag", longPressGesture.VelocityX, longPressGesture.VelocityY);
        }
    }

    public void CheckOnPointerDown()
    {
        float x = 0.0f;
        float y = 0.0f;

        // 일단 왼쪽 버튼에 대해서만
        if (Input.mousePresent && Input.GetMouseButtonDown(0))
        {
            x = Input.mousePosition.x;
            y = Input.mousePosition.y;
        }// 일단 한개의 터치에 대해서만.
        else if (Input.touchSupported && Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == UnityEngine.TouchPhase.Began)
            {
                x = t.position.x;
                y = t.position.y;
            }
            else
            {
                return;
            }

        }
        else
        {
            return;
        }

        MessageSystem.Instance.BroadcastSystems(this, "PointerDown", x, y, longPressureDuration);
    }

    public void CheckOnPointerUp()
    {
        float x = 0.0f;
        float y = 0.0f;

        // 일단 왼쪽 버튼에 대해서만
        if (Input.mousePresent && Input.GetMouseButtonUp(0))
        {
            x = Input.mousePosition.x;
            y = Input.mousePosition.y;
        }// 일단 한개의 터치에 대해서만.
        else if (Input.touchSupported && Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == UnityEngine.TouchPhase.Canceled || t.phase == UnityEngine.TouchPhase.Ended)
            {
                x = t.position.x;
                y = t.position.y;
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }

        MessageSystem.Instance.BroadcastSystems(this, "PointerUp", x, y);
    }
}
