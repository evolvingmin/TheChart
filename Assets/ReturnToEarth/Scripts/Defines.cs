using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    public static class Defines
    {
        public enum SlotType
        {
            weapon,
            body,
            utility
        }

        public enum WeaponType
        {
            missile,
            laser,
            mass
        }

        public enum EnemyType
        {
            Minion,
            MiddleBoss,
            Boss
        }

    }
}

