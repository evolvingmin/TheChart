using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip.UI
{
    public class WeaponItemController : MonoBehaviour
    {
        public enum Mode
        {
            Updated,
            Dirty
        }

        [SerializeField]
        private GameObject prefab = null;

        [SerializeField]
        private Transform content = null;

        private List<WeaponItem> uiItemList = new List<WeaponItem>();

        private Mode mode = Mode.Dirty;
        
        public bool Initialize()
        {
            if (mode == Mode.Updated)
                return false;

            var table = TableHandler.Get("OwnedWeapons", TableHandler.StreamMode.AppData);
            prefab.SetActive(true);
            foreach (var itemInfo in table.Rows)
            {
                WeaponItem weaponItem = Instantiate(prefab).GetComponent<WeaponItem>();
                weaponItem.transform.SetParent(content);
                weaponItem.transform.Reset();
                weaponItem.Initialize(itemInfo);
                uiItemList.Add(weaponItem);
            }
            prefab.SetActive(false);
            mode = Mode.Updated;
            return true;
        }

        public bool ClearItems()
        {
            foreach (var item in uiItemList)
            {
                DestroyImmediate(item.gameObject);
            }
            uiItemList.Clear();
            mode = Mode.Dirty;
            return true;
        }


        public void UpdateSelectInfo(WeaponItem selected)
        {
            foreach (var item in uiItemList)
                item.SetSelected(selected == item);
        }

        public void UpdateSelectInfo(TableHandler.Row selectedWeaponTable)
        {
            foreach (var item in uiItemList)
                item.SetSelected(selectedWeaponTable == item.Table);
        }
    }

}
