using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    public class BattleModifier
    {
        // 이런 Statues 데이터는 적과 플레이어가 공유한다
        // 즉 플레이어에 CriticalRate가 있으면, 적에게도 어떤식으로 계산하는지는 관심없고, 데이터는 존재해야 한다.
        public class Statues
        {
            public int damage;
            public float attackSpeed;
            public float bulletSpeed;
            public BulletType bulletType;
            public float criticalRate;
            public float accuracy;
            public float dropRate;
        }

        public Statues StatuesInfo { get { return statues; } }
        protected Statues statues;

        // 유저의 무기 하나에서 초기화 하는 데이터.
        public void GenerateDataFromWeapon(DataProvider dataProvider)
        {
            statues = new Statues()
            {
                damage = (int)dataProvider.GetStat("damage"),
                bulletSpeed = dataProvider.GetStat("bulletspeed"),
                attackSpeed = dataProvider.GetStat("attackspeed"),
                bulletType = (BulletType)Enum.Parse(typeof(BulletType), dataProvider.Get<string>("bullettype"), true),
                criticalRate = dataProvider.GetStat("criticalrate"),
                accuracy = dataProvider.GetStat("accuracy")//,
                //dropRate = dataProvider.GetStat("droprate")
            };
                        
        }

        // 적의 테이블에서 초기화 하는 데이터.
        public void GenerateDataFromEnemy(MonsterInfoTable.Row table)
        {
            statues = new Statues()
            {
                damage = table.i_attack,
                attackSpeed = table.f_attackSpeed,
                bulletSpeed = table.f_bulletSpeed,
                bulletType = BulletType.Mass,
                criticalRate = 0,
                accuracy = 100
            };
        }
    }
}