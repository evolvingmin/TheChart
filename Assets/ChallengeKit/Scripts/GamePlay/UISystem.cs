using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChallengeKit.GamePlay;

namespace ChallengeKit.Pattern
{

    public class UISystem : SystemMono
    {
        private Dictionary<string, UIComponent> uiBindings;

        private void Awake()
        {
            uiBindings = new Dictionary<string, UIComponent>();
            base.Init(new UIParser());
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public UIComponent GetUIComponent(string name)
        {
            if (uiBindings.ContainsKey(name))
            {
                return uiBindings[name];
            }
            else
            {
                UIComponent target = transform.Find(name).GetComponent<UIComponent>();

                if(target != null)
                {
                    uiBindings.Add(name, target);
                }
                else
                {
                    Debug.LogWarning("Finding UIComponent Failed, name is " + name);
                }

                return target;
            }
        }


    }


    public class UIParser : IParser
    {
        UISystem uiSystem;

        public Define.Result Init(SystemMono parentSystem)
        {
            uiSystem = (UISystem)parentSystem;

            return Define.Result.OK;
        }

        public void ParseCommand(string Command, params object[] Objs)
        {
            //MessageSystem.Instance.BroadcastSystems(null, "SetActive", "CandleDataDisplayer", bOpen);


            switch (Command)
            {
                case "SetActive":
                    uiSystem.GetUIComponent((string)Objs[0]).gameObject.SetActive((bool)Objs[1]);
                    break;
                case "InvalidateUI":
                    uiSystem.GetUIComponent((string)Objs[0]).InvalidateUI(Objs);
                    break;
                default:
                    break;
            }

        }

    }
}


