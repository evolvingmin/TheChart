using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    public class BattleObject
    {
        public class Statues
        {
            public int hp;
            public int defense;
            public float avoidRate;
            public int currentHP;
            public float dropRate;
            public float dropRateSupplies;
            public float dropRateRearmetals;
            public float dropRateBioMetals;
            //public int dropQuality;
            //public int dropAmount;
        }

        public Statues StatuesInfo { get { return statues; } }
        protected Statues statues;

        public virtual void Initialize(DataProvider dataProvider)
        {
            //신경 안써도 댐
        }

        public virtual void Initialize(MonsterInfoTable.Row table, Enemy enemy)
        {
            //신경 안써도 댐
        }

        public virtual bool IsHit(BattleModifier bulletModifier)
        {
            int randomseed; 
            bool result;
            float avoidRate;
            float accuracy;

            accuracy = bulletModifier.StatuesInfo.accuracy;
            avoidRate = statues.avoidRate;

            if ((accuracy - avoidRate) >= 100)
            {
                result = true;
            }
            else
            {
                randomseed = Random.Range(0, 100);
                if ( randomseed > ( accuracy - avoidRate ) ) {
                    result = true;
                } else {
                    result = false;
                }


            }
            
            


            return  result;
        }

        public virtual void TakeTrueDamage(int Damage)
        {
            //실제 데미지를 입는곳


            statues.currentHP -= Damage;
            if (statues.currentHP <= 0)
            {
                statues.currentHP = 0;
            }
        }

        public virtual int GetActualDamage(BattleModifier bulletModifier)
        {
            //실제 데미지를 계산하는곳
            /* 
             		명중률은 100% 이상 증가할 수 있다.										
		명중시 크리 판정을 하여 치명타가 터진다.										
		명중판정은 명중률 - 회피율이다.										
		명중 판정 후 명중률 - 회피율을 적용하여 100% 이상이 나오면 기본 2배의 데미지가 들어간다.(치명타)										
			치명타시 치명데미지 증가 능력치에 따라 변화할 수 있다									
		명중률 - 회피율을 적용하여 50% 이하라면 절반의 데미지가 들어간다.(역치명타)										
		명중시 데미지 판단 공식은 데미지 계산값 * 치명 혹은 역치명										

             
             
             */
            int absDefence;
            float relatDefence;
            int finalDamage;
            float defence = statues.defense;
            float avoidRate = statues.avoidRate;
            float accuracy = bulletModifier.StatuesInfo.accuracy;
            int damage = bulletModifier.StatuesInfo.damage;
            float damageMulti = 1.0f;
            bool check;
            float criticalRate = bulletModifier.StatuesInfo.criticalRate;
            
            //명중률에 따른 데미지배수 조정
            if ( ( accuracy - avoidRate ) < 51 ) {
                //if ( damageMulti > 1 ) {
                //    damageMulti = damageMulti - 1;
                //} else {
                    damageMulti = damageMulti * 0.5f;
                //}
            } else {
                do {
                    if ( ( accuracy - avoidRate ) > 99 ) {
                        damageMulti = damageMulti + 1;
                        accuracy = accuracy - 100;
                        check = true;
                    } else {
                        check = false;
                    }
                } while ( check );
            }

            
            //크리율에 따른 데미지 배수 조정
            do {
                if ( criticalRate > 99 ) {
                    check = true;
                    damageMulti = damageMulti + 1;
                    criticalRate = criticalRate - 100;
                } else {
                    check = false;
                    if ( criticalRate > Random.Range(0, 100) ) {
                        damageMulti = damageMulti + 1;
                    }
                }
            } while (check);






            //이거 실제 데미지 입을때 계산해야 할듯
            //여기서는 데미지가 얼마 들어올지만 계산하는걸로 한다.


            //절대 및 상대 방어력 계산
            absDefence = (int)( defence / 4 );
            relatDefence = Mathf.Log((float)defence,4.0f) * 0.1f;


            //최종 데미지 조정
            finalDamage = (int)((damage*damageMulti) * (1-relatDefence)) - absDefence;
            //현재 크리 데미지가 반영은 되어 있음. 다만 크리데미지 팩터가 추가된다면 수정해야댐

            return Mathf.Max(0, finalDamage);
        }
    }
}
