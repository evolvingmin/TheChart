using System.Collections;
using System.Collections.Generic;
using ChallengeKit;
using ChallengeKit.Pattern;
using UnityEngine;

public class EconomyParser : IParser
{
    EconomySystem economySystem;

    public Define.Result Init(SystemMono parentSystem)
    {
        economySystem = (EconomySystem)parentSystem;

        return Define.Result.OK;
    }

    public bool ParseCommand(string Command, params object[] Objs)
    {
        //MessageSystem.Instance.BroadcastSystems(null, "SetActive", "CandleDataDisplayer", bOpen);
        switch (Command)
        {
            case "StartTransection":
                economySystem.StartTransection((bool)Objs[0], (float)Objs[1]);
                return true;

        }

        return false;
    }
}

public class EconomySystem : SystemMono
{
    [SerializeField]
    private Chart chart;

    [SerializeField]
    private float basePercent = 0.05f;

    private void Awake()
    {
        base.Init(new EconomyParser());
    }

    // 오늘 그냥 이 부분 미완성으로 차트에 영향만 끼치는 거 영상으로 넣겠다.
    public void StartTransection(bool bUp, float Scale)
    {
        float affectPercent = basePercent * ( bUp ? 1 : -1 ) * Scale;

        chart.AddTransection(affectPercent);
    }
}
