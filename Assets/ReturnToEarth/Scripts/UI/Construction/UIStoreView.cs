using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip.UI
{
    public class UIStoreView : MonoBehaviour
    {
        [SerializeField]
        private Transform content;

        [SerializeField]
        private GameObject prefab;

        private Dictionary<UIStore.Category, List<UIStoreItem>> uiBlueprintList;
        private Dictionary<UIStore.Category, List<UIStoreItem>> uiEquipmentList;

        private Dictionary<UIStore.Category, List<UIStoreItem>> currentCollection;
        //private List<UIStoreItem> uiStoreItem = new List<UIStoreItem>();
        private UIStore uiStore = null;
        private UIStore.Category currentCategory;
        private UIStore.Mode currentMode;

        public bool Initialize(UIStore uiStore, UIStore.Category category)
        {
            this.uiStore = uiStore;
            uiEquipmentList = new Dictionary<UIStore.Category, List<UIStoreItem>>();
            uiBlueprintList = new Dictionary<UIStore.Category, List<UIStoreItem>>();

            // 이 코드는 순서 종속성이 있다. 코드 수정이 필요함....
            InitializeStoreItems(UIStore.Category.Hull, UIStore.Mode.Equipment);
            InitializeStoreItems(UIStore.Category.Utilities, UIStore.Mode.Equipment);
            InitializeStoreItems(UIStore.Category.Weapon, UIStore.Mode.Equipment);

            InitializeStoreItems(UIStore.Category.Hull, UIStore.Mode.BluePrint);
            InitializeStoreItems(UIStore.Category.Utilities, UIStore.Mode.BluePrint);
            InitializeStoreItems(UIStore.Category.Weapon, UIStore.Mode.BluePrint);
            
            ChangeMode(UIStore.Mode.Equipment);
            ActiveCategoryItems(category);

            return true;
        }

        public bool ChangeMode(UIStore.Mode mode)
        {
            UIStore.Category current = currentCategory;
            ActiveCategoryItems(UIStore.Category.None);
            switch (mode)
            {
                case UIStore.Mode.Equipment:
                    currentCollection = uiEquipmentList;
                    break;
                case UIStore.Mode.BluePrint:
                    currentCollection = uiBlueprintList;
                    break;
            }
            ActiveCategoryItems(current);
            return true;
        }

        public bool ActiveCategoryItems(UIStore.Category category)
        {
            foreach (var Collection in currentCollection.Values)
            {
                foreach (var item in Collection)
                {
                    item.gameObject.SetActive(item.Category == category);
                }
            }
            currentCategory = category;
            return true;
        }

        private bool InitializeStoreItems(UIStore.Category category, UIStore.Mode mode)
        {
            TableHandler.Table table = null;
            TableHandler.Table ownedTable = null;
            switch (category)
            {
                case UIStore.Category.Weapon:
                    table = TableHandler.Get("WeaponList", TableHandler.StreamMode.Resource);
                    ownedTable = TableHandler.Get("OwnedBluePrintWeapons",TableHandler.StreamMode.AppData);
                    break;
                case UIStore.Category.Hull:
                    table = TableHandler.Get("HullList", TableHandler.StreamMode.Resource);
                    ownedTable = TableHandler.Get("OwnedBluePrintHulls", TableHandler.StreamMode.AppData);
                    break;
                case UIStore.Category.Utilities:
                    break;
            }

            if (table == null)
                return false;

            switch (mode)
            {
                case UIStore.Mode.Equipment:
                    currentCollection = uiEquipmentList;
                    break;
                case UIStore.Mode.BluePrint:
                    currentCollection = uiBlueprintList;
                    
                    break;
            }

            if (!currentCollection.ContainsKey(category))
            {
                currentCollection.Add(category, new List<UIStoreItem>());
                prefab.SetActive(false);

                foreach (var row in table.Rows)
                {
                    if (mode == UIStore.Mode.Equipment)
                    {
                        var result = ownedTable.FindRow("blueprintIndex", row.Get<int>("index"));
                        if (result == null)
                            continue;
                    }

                    UIStoreItem storeItem = Instantiate(prefab).GetComponent<UIStoreItem>();
                    storeItem.transform.SetParent(content);
                    storeItem.transform.Reset();
                    storeItem.Initialize(uiStore, row, category, mode);
                    currentCollection[category].Add(storeItem);

                    if (mode == UIStore.Mode.BluePrint)
                    {
                        var result = ownedTable.FindRow("blueprintIndex", row.Get<int>("index"));
                        if (result != null)
                        {
                            storeItem.SetSold();
                        }
                    }

                }
            }

            currentCategory = category;
            return true;
        }

        private void AddNewEquipment(UIStore.Category category)
        {
            TableHandler.Table table = null;
            TableHandler.Table ownedTable = null;
            switch (category)
            {
                case UIStore.Category.Weapon:
                    table = TableHandler.Get("WeaponList", TableHandler.StreamMode.Resource);
                    ownedTable = TableHandler.Get("OwnedBluePrintWeapons", TableHandler.StreamMode.AppData);
                    break;
                case UIStore.Category.Hull:
                    table = TableHandler.Get("HullList", TableHandler.StreamMode.Resource);
                    ownedTable = TableHandler.Get("OwnedBluePrintHulls", TableHandler.StreamMode.AppData);
                    break;
                case UIStore.Category.Utilities:
                    // Not implemented
                    break;
            }

            foreach (var row in table.Rows)
            {
                var result = ownedTable.FindRow("blueprintIndex", row.Get<int>("index"));
                if (result == null)
                    continue;

                if (uiEquipmentList[category].Find(x => x.Row == row))
                    continue;
                
                UIStoreItem storeItem = Instantiate(prefab).GetComponent<UIStoreItem>();
                storeItem.transform.SetParent(content);
                storeItem.transform.Reset();
                storeItem.Initialize(uiStore, row, category, UIStore.Mode.Equipment);
                uiEquipmentList[category].Add(storeItem);
                storeItem.gameObject.SetActive(false);
            }

        }

        public void UpdateUI()
        {
            AddNewEquipment(currentCategory);
        }

        // 블루프린트를 이용해여 Owned아이템을 만든다. 
        public static Dictionary<string, string> GenerateItemByBluePrint(TableHandler.Row row, UIStore.Category category)
        {
            string targetTableName = "";
 
            // 하 드 코 딩
            switch (category)
            {
                case UIStore.Category.Weapon:
                    targetTableName = "OwnedWeapons";
                    break;
                case UIStore.Category.Hull:
                    targetTableName = "OwnedHulls";
                    break;
                case UIStore.Category.Utilities:
                    targetTableName = "OwnedUtilitys";
                    break;
            }

            var SampleTable = TableHandler.Get(targetTableName, TableHandler.StreamMode.Sample);
            var StatQueryed = row.QueryByContainedString<string>("_SB");

            // 해 주어야 하는 작업 : _SC 에 랜덤 추가 값을 넣어주고 돌려주기.
            //SampleTable.colu
            List<string> NewData = new List<string>(SampleTable.ColumnHeader.Count);
            for (int i = 0; i < SampleTable.ColumnHeader.Count; i++)
            {
                NewData.Add("0");
            }
            NewData[1] = row.Get<string>("index");

            foreach (var item in StatQueryed)
            {
                int TargetIndex = SampleTable.GetIndex(item.Key.Replace("SB", "SC"));
                if (TargetIndex == -1)
                    continue;
                
                int[] Range = Array.ConvertAll(item.Value.Split(','), Parsed => Convert.ToInt32(Parsed));

                if(Range.Length >=2)
                {
                    NewData[TargetIndex] = Convert.ToString(UnityEngine.Random.Range(Range[0], Range[1]) - Range[0]);
                }
                else
                {
                    NewData[TargetIndex] = "0";
                }
            }

            Dictionary<string, string> generated = new Dictionary<string, string>();
            var keyEnumerator = SampleTable.ColumnHeader.Keys.GetEnumerator();

            int index = 0;
            while (keyEnumerator.MoveNext())
            {
                var current = keyEnumerator.Current;
                generated.Add(current, NewData[index]);
                index++;
            }

            return generated;
        }

        public void UpdateSelected(UIStoreItem current)
        {
            foreach (var item in currentCollection[currentCategory])
            {
                item.SetSelected(current == item);
            }
        }

    }

}
