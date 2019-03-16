
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRubyShared;
using ChallengeKit.Pattern;


public class ChartInputSystem : SystemMono
{
    private TapGestureRecognizer tapGesture;
    private LongPressGestureRecognizer      longPressGesture;
    //private SwipeGestureRecognizer          swipeGesture;

    // Start is called before the first frame update
    void Start()
    {
        CreateTapGesture();
        //CreateSwipeGesture();
        CreateLongPressGesture();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
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
    */

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
            MinimumDurationSeconds = 0.2f
        };
        longPressGesture.StateUpdated += LongPressGestureCallback;
        FingersScript.Instance.AddGesture(longPressGesture);
    }

    private void TapGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            //DebugText("Tapped at {0}, {1}", gesture.FocusX, gesture.FocusY);
            //CreateAsteroid(gesture.FocusX, gesture.FocusY);
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


}
