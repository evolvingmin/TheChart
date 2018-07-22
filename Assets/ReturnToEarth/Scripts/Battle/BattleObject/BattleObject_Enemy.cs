using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    public class BattleObject_Enemy : BattleObject {

        //적에게만 적용하고 싶은 효과들에 대한 항목

        protected Enemy currentEnemy;
        public WealthManager wealthManager;

        public override void Initialize(MonsterInfoTable.Row table, Enemy enemy)
        {
            statues = new Statues() {
                defense = table.i_defence,
                avoidRate = 0,
                currentHP = table.i_hp,
                hp = table.i_hp,
                dropRate = table.f_dropRate,
                dropRateSupplies = table.f_dropRateSupplies,
                dropRateRearmetals = table.f_dropRateRearmetals,
                dropRateBioMetals = table.f_dropRateBioMetals
                //dropQuality = 33,
                //dropAmount = 5

            };

            wealthManager = new WealthManager();

            currentEnemy = enemy;
        }

        public override void TakeTrueDamage(int Damage)
        {
            base.TakeTrueDamage(Damage);

            if (statues.currentHP == 0 && currentEnemy != null)
            {
                float DChance = Random.Range(0f, 100f);
                if ( DChance < statues.dropRate ) {
                    
                    wealthManager.IncreaseDebris(); //이걸로 저쪽에 데브리 양 증가 시킨다. 증가량은 어찌 계산하지?

                } else {
                    Debug.Log("DropChance = " + DChance);
                }
                                
                currentEnemy.Collect();
            }
        }
    }

}
