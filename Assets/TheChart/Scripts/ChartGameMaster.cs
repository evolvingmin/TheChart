using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChallengeKit.Pattern;
using ChallengeKit;
using System;

public class ChartGMParser : IParser
{
    ChartGameMaster gmSystem;

    public Define.Result Init(SystemMono parentSystem)
    {
        gmSystem = (ChartGameMaster)parentSystem;

        return Define.Result.OK;
    }

    public bool ParseCommand(string Command, params object[] Objs)
    {
        //MessageSystem.Instance.BroadcastSystems(null, "SetActive", "CandleDataDisplayer", bOpen);
        switch (Command)
        {
            case "BeginDrag":
                gmSystem.BeginDrag((float)Objs[0], (float)Objs[1]);
                return true;
            case "EndDrag":
                gmSystem.EndDrag((float)Objs[0], (float)Objs[1]);
                return true;
        }

        return false;
    }
}

public class ChartGameMaster : SystemMono 
{
    [SerializeField]
    private float timeSlowPerTick = 0.0002f;


    private bool bSlowing = false;

    private Coroutine slowingCorutine = null;

    private ResourceManager resourceManager;

    private EconomySystem economySystem;

    private void Awake()
    {
        base.Init(new ChartGMParser());
        resourceManager = GetComponent<ResourceManager>();
        economySystem = GetComponent<EconomySystem>();

        resourceManager.Init();
        //economySystem.Init();
        Chart.Instance.Init();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BeginDrag(float v1, float v2)
    {
        if (bSlowing)
        {
            StopSlowing();
        }
           
        StartSlowing();
    }

    public void EndDrag(float v1, float v2)
    {
        StopSlowing();
    }

    private void StartSlowing()
    {
        bSlowing = true;
        slowingCorutine = StartCoroutine(Slowing());
    }

    private void StopSlowing()
    {
        if(slowingCorutine != null)
        {
            Time.timeScale = 1.0f;
            StopCoroutine(slowingCorutine);
            bSlowing = false;
        }
    }

    IEnumerator Slowing()
    {
        float astimatedScale = Time.timeScale;

        while (astimatedScale > 0 )
        {
            astimatedScale -= timeSlowPerTick;

            if(astimatedScale >= 0)
            {
                Time.timeScale -= timeSlowPerTick;
            }
            //Debug.Log("Current Scale " + Time.timeScale);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }

        Time.timeScale = 0.0f;
        bSlowing = false;
    }


}
