using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChallengeKit.Pattern;
using ChallengeKit;

public class ChartIndicateParser : IParser
{
    ChartIndicatorSystem chartIndicatorSystem;

    public Define.Result Init(SystemMono parentSystem)
    {
        chartIndicatorSystem = (ChartIndicatorSystem)parentSystem;

        return Define.Result.OK;
    }

    public bool ParseCommand(string Command, params object[] Objs)
    {
        //MessageSystem.Instance.BroadcastSystems(null, "SetActive", "CandleDataDisplayer", bOpen);
        switch (Command)
        {
            case "HandleSwipe":
                chartIndicatorSystem.HandleSwipe((float)Objs[0], (float)Objs[1], (float)Objs[2], (float)Objs[3], (float)Objs[4], (float)Objs[5]);
                return true;
            case "BeginDrag":
                chartIndicatorSystem.BeginDrag((float)Objs[0], (float)Objs[1]);
                return true;
            case "DragTo":
                chartIndicatorSystem.DragTo((float)Objs[0], (float)Objs[1]);
                return true;
            case "EndDrag":
                chartIndicatorSystem.EndDrag((float)Objs[0], (float)Objs[1]);
                return true;
        }

        return false;
    }
}


public class ChartIndicatorSystem : SystemMono
{
    [SerializeField]
    private List<UIComponent> uiComponents;

    private void Awake()
    {
        base.Init(new ChartIndicateParser());
    }

    public void HandleSwipe(float startX, float startY, float endX, float endY, float velocityX, float velocityY)
    {
        foreach (var uiComponent in uiComponents)
        {
            uiComponent.HandleSwipe(startX, startY,endX, endY, velocityX, velocityY);
        }
    }

    public void BeginDrag(float positionX, float positionY)
    {
        foreach (var uiComponent in uiComponents)
        {
            uiComponent.BeginDrag(positionX, positionY);
        }
    }

    public void DragTo(float positionX, float positionY)
    {
        foreach (var uiComponent in uiComponents)
        {
            uiComponent.DragTo(positionX, positionY);
        }
    }

    public void EndDrag(float velocityX, float velocityY)
    {
        foreach (var uiComponent in uiComponents)
        {
            uiComponent.EndDrag(velocityX, velocityY);
        }
    }
}
