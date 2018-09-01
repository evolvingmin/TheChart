using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarShip.UI
{
    public class WeaponItem : MonoBehaviour
    {
        [SerializeField]
        private new Text name = null;
        [SerializeField]
        private Button button = null;
        [SerializeField]
        private Image selected = null;
        [SerializeField]
        private Image icon = null;

        private TableHandler.Row table = null;
        public TableHandler.Row Table
        {
            get { return table;  }
        }
        private DataProvider dataProvider;


        public bool Initialize(TableHandler.Row table)
        {
            this.table = table;
            dataProvider = new DataProvider("WeaponList", table);

            name.text = dataProvider.Get<string>("name");
            button.onClick.AddListener(WeaponSelected);
            icon.sprite = Resources.Load<Sprite>(string.Format("Image/Slot/Weapon/{0}/{1}",
                dataProvider.Get<string>("weapontype"),
                dataProvider.Get<string>("icon")));

            return true;
        }

        //선택하면 현재 선택한 아이템에 대한 정보를 가지고 있는곳이 필요하다.
        public void WeaponSelected()
        {
            ConstructionController.Instance.UI.Conbay.SlotSelect.UpdateSelectedItem(table,this);
        }

        public void SetSelected(bool isSelected)
        {
            selected.gameObject.SetActive(isSelected);
        }
    }
}

