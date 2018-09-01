using System;
using System.Collections.Generic;
using UnityEngine;

// --------------------------------------주 의 사항-------------------------------------------------------
// 지금은 Resources 있는 테이블을 로드하지만, 나중에는 이 데이터들을 플레이어 프렙이나 서버로 옮겨 사용해야 한다.
// 구현의 쉬움을 위해서 Resources 에 넣어두지만, Resources 있으면 모바일 플레이 시 정상작동을 보장하지 않는다.
// csvTablePro의 기능을 이 현 프로젝트의 요구사항에 맞춰서 완전 변경하도록 한다. 

// 남은 할 것 : 최초 스타십 로드 할 때 로드된 테이블을 바탕으로 slotList에 아이템 구성하기 (바디는 따로 저장)

namespace StarShip
{
    // 이런 인터페이스를 바로 여기서 상속하지 않고, IStarShipObject 를 상속한, 혹은 인터페이스 사용하지 않은
    // 비슷한방식의 StartShipObjectInfo를 구현하면 된다. 
    // todo : SSObjectInfo 로 빼고 여기에 TakeDamge, GetActual Defense 등등을 구현하도록 하자.
    public class StarShip : MonoBehaviour
    {
        public enum Mode
        {
            Battle,
            Construction
        }

        [SerializeField]
        private List<WeaponSlot> slotList = null;

        [SerializeField]
        private GameObject hpTextPrefab = null;
        private PlayerHPText playerHPText = null;

        public Mode StarShipMode { get { return mode; } }
        private Mode mode = Mode.Battle;

        private DataProvider dataProvider;

        public BattleObject BattleObject { get { return battleObject; } }
        private BattleObject battleObject;

        public bool Initialize(Mode mode, DataProvider dataProvider)
        {
            this.mode = mode;

            TableHandler.Table shiptStatuesTable = TableHandler.Get("ShipStatus", TableHandler.StreamMode.AppData);

            string preParse = shiptStatuesTable.Rows[0].Get<string>("weapon");

            if (preParse != "")
            {
                int[] weaponIndexs = Array.ConvertAll(preParse.Split(','), item => Convert.ToInt32(item));

                TableHandler.Table availableWeapons = TableHandler.Get("OwnedWeapons", TableHandler.StreamMode.AppData);

                for (int i = 0; i < weaponIndexs.Length; i++)
                {
                    TableHandler.Row matchedWeapon = availableWeapons.FindRow("index", weaponIndexs[i]);
                    if (matchedWeapon == null)
                        continue;

                    slotList.Find(x => ( x.slotType == Defines.SlotType.weapon && x.slotIndex == i + 1 )).Initialize(this, matchedWeapon);
                }
            }

            this.dataProvider = dataProvider;
            battleObject = new BattleObject_Starship();
            battleObject.Initialize(this.dataProvider);

            switch (mode)
            {
                case Mode.Battle:
                    playerHPText = Instantiate(hpTextPrefab).GetComponent<PlayerHPText>();
                    playerHPText.Initialize(this);
                    GetComponent<Collider2D>().enabled = true;
                    break;
                case Mode.Construction:
                    GetComponent<Collider2D>().enabled = false;
                    break;
            }

            return true;
        }

        public void UpdateSlotInfo(string hullIndex, string weaponIndex, int slotIndex = 0)
        {
            var table = TableHandler.Get("ShipStatus", TableHandler.StreamMode.AppData);

            List<string> columns = new List<string>(table.ColumnHeader.Count)
            {
                (slotIndex + 1).ToString(),hullIndex.ToString(),weaponIndex
            };

            table.GetAt(slotIndex).Replace(columns);
            TableHandler.Save("ShipStatus");

        }

        public void SaveCurrentDataAsCSV(int slotIndex)
        {
            var table = TableHandler.Get("ShipStatus", TableHandler.StreamMode.AppData);

            // 종합 배의 상태를 저장하기 이전에 보유하고 있는 데이터 역시 갱신 해 준다.
            // 밑관련으로 종합하는 함수 필요할듯?
            TableHandler.Get("OwnedWeapons", TableHandler.StreamMode.AppData);
            TableHandler.Save("OwnedWeapons");
            TableHandler.Get("OwnedHulls", TableHandler.StreamMode.AppData);
            TableHandler.Save("OwnedHulls");
            TableHandler.Get("OwnedUtilitys", TableHandler.StreamMode.AppData);
            TableHandler.Save("OwnedUtilitys");

            //임시 테이블
            //GameWealth.Start("Table/GameWealth");            


            string hullIndex = table.GetAt(slotIndex).Get<string>("hull");
            string weaponIndex = "";

            for (int i = 0; i < slotList.Count; i++)
            {
                if (slotList[i] == null)
                    continue;

                weaponIndex += slotList[i].Row.Get<string>("index");

                if (i != ( slotList.Count - 1 ))
                    weaponIndex += ",";
            }

            UpdateSlotInfo(hullIndex, weaponIndex, slotIndex);
        }

        public void NotifyCurrentEnemyDead()
        {
            foreach (var item in slotList)
            {
                if (item != null)
                    item.NotifyCurrentEnemyDead();
            }
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Bullet"))
            {
                return;
            }

            Bullet bullet = collision.GetComponent<Bullet>();

            if (bullet.ShooterTag != "Enemy")
                return;

            if(bullet.Hit(battleObject))
            {
                battleObject.TakeTrueDamage(battleObject.GetActualDamage(bullet.Modifier));
            }
        }

        public static bool GenerateStarShip(Mode mode, out StarShip starship, int slotIndex )
        {
            string path = "Prefabs/StarShip/{0}";
            // 현재의 ShipStatus를 먼저 읽고 나서
            // 그 ShipStatus에서 정의한 프리펩을 로드해야 함.
            TableHandler.Row shipInfo = TableHandler.Get("ShipStatus", TableHandler.StreamMode.AppData).GetAt(slotIndex);
            TableHandler.Row hullInfo = TableHandler.Get("OwnedHulls", TableHandler.StreamMode.AppData).FindRow("index", shipInfo.Get<string>("hull"));

            DataProvider hullListProvider = new DataProvider("HullList", hullInfo);
            string prefabName = hullListProvider.Get<string>("targetprefab");
            var prefab = Resources.Load(string.Format(path, prefabName));

            starship = ( (GameObject)Instantiate(prefab) ).GetComponent<StarShip>();
            return starship.Initialize(mode, hullListProvider);
        }

        // 임시로 넘길 카드리스트, 이 리스트는 무기에서 완성됨
        public List<string> GetCardList()
        {
            List<string> temp = new List<string>
            {
                "TestBomb",
                "TestBomb",
                "TestBomb",
                "TestBomb",
                "TestBomb",
                "TestBomb",
                "TestBomb",
                "TestBomb"
            };

            return temp;
        }
    }

}
