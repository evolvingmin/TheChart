using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    public class BattleObject_Starship : BattleObject
    {
        public override void Initialize(DataProvider dataProvider)
        {
            statues = new Statues()
            {
                defense = (int)dataProvider.GetStat("defense"),
                avoidRate = (int)dataProvider.GetStat("avoidrate"),
                currentHP = (int)dataProvider.GetStat("hp"),
                hp = (int)dataProvider.GetStat("hp"),
            };
        }
    }
}


