using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarShip.UI
{ 
    public class UIShipItem : MonoBehaviour
    {
        [SerializeField]
        private Image icon;
        [SerializeField]
        private Text label;
        [SerializeField]
        private GameObject Selected;
        [SerializeField]
        private Button button;

        public TableHandler.Row Row {  get { return row; } }
        private TableHandler.Row row;
        private DataProvider dataProvider;

        public bool Initialize(TableHandler.Row item)
        {
            row = item;
            dataProvider = new DataProvider("HullList", item);

            button.onClick.AddListener(ItemSelected);
            label.text = dataProvider.Get<string>("name");
            icon.sprite = Resources.Load<Sprite>(string.Format("Image/Ship/{0}", dataProvider.Get<string>("icon")));
            gameObject.SetActive(true);
            return true;
        }

        public void ItemSelected()
        {
            // 현재 선택된 쉽을 재 초기화 해서 불러오는 로직
            ConstructionController.Instance.UI.Conbay.ShipSelect.SetCurrent(this);
        }

        public void SetSelected(bool value)
        {
            Selected.SetActive(value);
            button.interactable = !value;
        }
    }
}
