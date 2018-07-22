using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarShip.UI
{
    // 내가 가지고 있는 함선 리스트를 UI 리스트로 표시하고,
    // 선택할 경우, 현재 선택된 함선을 변경하는 기능, 
    // 구현해야 할 것
    // Starship 레이지케싱 시스템, (만들지 말 것)
    // 프로토 타입으로 스타쉽은 그 즉시즉시 Instantiate 하게 만들 것 (나중에는 최적화 이슈로 케싱 시스템 만들것임)

    public class UIShipSelect : MonoBehaviour
    {
        public enum Mode
        {
            Dirty,
            Set
        }


        public GameObject root;

        private UIConbay uiconbay;

        [SerializeField]
        private Transform content;
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private Button confirm;

        private UIShipItem currentItem = null;
        private int currentIndex;
        private Mode mode = Mode.Dirty;

        public bool Initialize(UIConbay uiconbay)
        {
            this.uiconbay = uiconbay;
            GenerateShipItem();
            root.SetActive(false);
            return true;
        }

        public void Active(bool value)
        {
            if(value)
            {
                if (mode == Mode.Dirty)
                    GenerateShipItem();

                root.SetActive(true);
                TableHandler.Row shipInfo = TableHandler.Get("ShipStatus", TableHandler.StreamMode.AppData).GetAt(0);
                currentIndex = shipInfo.Get<int>("hull");
                SetCurrent(currentIndex);
            }
            else
            {
                root.SetActive(false);
                uiconbay.BackToMain();
            }
        }

        private List<UIShipItem> shipItems = new List<UIShipItem>();

        public void SetCurrent(int hullIndex)
        {
            var Selected = shipItems.Find(x => x.Row.Get<int>("index") == hullIndex);
            SetCurrent(Selected);
        }

        public void SetCurrent(UIShipItem selected)
        {
            currentItem = selected;
            foreach (var item in shipItems)
            {
                item.SetSelected(item == selected);
            }

            confirm.interactable = currentIndex != selected.Row.Get<int>("index");
        }

        public void OnConfirm()
        {
            // 현재 선택된 배를 교체하고, 
            confirm.interactable = false;   
            currentIndex = currentItem.Row.Get<int>("index");
            ConstructionController.Instance.UpdateStarShip(currentIndex.ToString());
        }

        public bool ClearItems()
        {
            foreach (var item in shipItems)
            {
                DestroyImmediate(item.gameObject);
            }
            shipItems.Clear();
            mode = Mode.Dirty;
            return true;
        }


        public bool GenerateShipItem()
        {
            //private List<>

            TableHandler.Table table = TableHandler.Get("OwnedHulls", TableHandler.StreamMode.AppData);

            if (table == null)
                return false;

            prefab.SetActive(false);

            foreach (var item in table.Rows)
            {
                UIShipItem shiptItem = Instantiate(prefab).GetComponent<UIShipItem>();
                shiptItem.transform.SetParent(content);
                shiptItem.transform.Reset();
                shiptItem.Initialize(item);
                shipItems.Add(shiptItem);
            }
            mode = Mode.Set;
            return true;
        }
    }
}
