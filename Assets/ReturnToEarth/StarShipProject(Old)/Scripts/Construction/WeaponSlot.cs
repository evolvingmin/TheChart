using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    public class WeaponSlot : MonoBehaviour
    {
        private ConstructionController constructionController = null;

        private StarShip starShip = null;

        [SerializeField]

        private DataProvider dataProvider;
        private TableHandler.Row row = null;
        public TableHandler.Row Row
        {
            get { return row;}
        }

        public Defines.SlotType slotType = Defines.SlotType.weapon;

        // 현재 이 슬롯에 장착된 무기. 유틸리티나 함재기 모두 무기 취급한다, 일단은 생성하지 않고 그냥 부착해서 테스트 를 해 보자.
        [SerializeField]
        private Weapon equippedWeapon;

        public int slotIndex;

        public bool Initialize(StarShip starShip, TableHandler.Row selectedWeaponTable)
        {
            this.starShip = starShip;
            dataProvider = new DataProvider("WeaponList", selectedWeaponTable);
            switch (starShip.StarShipMode)
            {
                case StarShip.Mode.Battle:
                    UpdateSlot(selectedWeaponTable, Weapon.Mode.IDLE);
                    break;
                case StarShip.Mode.Construction:
                    constructionController = ConstructionController.Instance;
                    UpdateSlot(selectedWeaponTable, Weapon.Mode.OFF);
                    break;
            }

            return true;
        }

        public void FocusOnThis()
        {
            if (starShip.StarShipMode == StarShip.Mode.Battle)
                return;

            if(ConstructionController.Instance.UI.ConstructionMode == UI.ConstructionUIController.Mode.Conbay)
                constructionController.UI.Conbay.ChangeMode_SlotChange(this);
        }

        public void UpdateSlot(TableHandler.Row selectedWeaponTable, Weapon.Mode _mode = Weapon.Mode.OFF)
        {
            if(row != selectedWeaponTable)
            {
                row = selectedWeaponTable;
                dataProvider.Update(row);
            }

            // 테스트용 코드
            if(row.Get<string>("weapon_appearance") == "laser_turret" || row.Get<string>("weapon_appearance") == "mass_turret")
            {
                if (equippedWeapon != null)
                    DestroyImmediate(equippedWeapon.gameObject);

                equippedWeapon = Weapon.GetWeaponByPrefab(row);
                equippedWeapon.transform.SetParent(transform);
                equippedWeapon.transform.Reset();
            }
            equippedWeapon.Initialize(row, _mode);
            Debug.Log("weapon Equipped :" + dataProvider.Get<string>("name"));
        }

        public void NotifyCurrentEnemyDead()
        {
            if (equippedWeapon != null)
                equippedWeapon.NotifyCurrentEnemyDead();
        }
    }
}
