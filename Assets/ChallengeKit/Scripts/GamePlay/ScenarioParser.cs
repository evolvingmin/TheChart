using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChallengeKit.Pattern;

namespace ChallengeKit.GamePlay
{
    class ScenarioParser : IParser
    {
        ScenarioSystem scenarioSystem;

        public Define.Result Init(SystemMono parentSystem)
        {
            scenarioSystem = (ScenarioSystem)parentSystem;

            if (scenarioSystem == null)
                return Define.Result.NOT_INITIALIZED;

            return Define.Result.OK;
        }

        public void ParseCommand(string Command, params object[] Objs)
        {
            if (Command == "OnSelectionConfirm")
            {
                scenarioSystem.UpdateScenarioNode((int)Objs[0]);
            }
        }
    }
}
