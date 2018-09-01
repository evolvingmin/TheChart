using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// 샘플은 시작하자 마자 블루프린트에서 함수로 재작, 스텟은 범위에서 랜덤으로
// 일단은 무기만
// 목표는 OwnedWeapon을 갱신하는 것.
// Replace 말고 add 와 remove도 추가해야 한다. 
namespace StarShip.UI
{
    public class UIStore : MonoBehaviour
    {
        public enum State
        {
            None,
            Selected,
        }

        // 기본은 무기로 가정.
        public enum Category
        {
            None,
            Weapon,
            Hull,
            Utilities
        }

        public enum Mode
        {
            Equipment,
            BluePrint
        }

        [SerializeField]
        private GameObject root;

        [SerializeField]
        private UIStoreView uiStoreView;

        [SerializeField]
        private UIStoreItemDisplayer uiStoreItemDisplayer;

        [SerializeField]
        private Button buyButton;

        //private ConstructionUIController UI;

        private UIStoreItem current = null;

        public Mode CurrentMode {  get { return mode; } }
        private Mode mode = Mode.Equipment;
        private State state = State.None;
        private Category category = Category.Weapon;

        public bool Initialize(ConstructionUIController constructionUIController)
        {
            //UI = constructionUIController;
            root.SetActive(false);
            uiStoreView.Initialize(this, category);
            buyButton.interactable = false;
            return true;
        }

        public void Active(bool value)
        {
            root.SetActive(value);
        }

        public void UpdateSelected(UIStoreItem uIStoreItem)
        {
            current = uIStoreItem;
            uiStoreView.UpdateSelected(current);
            uiStoreItemDisplayer.UpdateSelected(current);

            if (current.ItemMode == UIStoreItem.Mode.Sold)
                return;

            buyButton.interactable = true;
            state = State.Selected;
        }

        public void Buy()
        {
            if (state == State.None)
                return;

            if (current.ItemMode == UIStoreItem.Mode.Sold)
                return;

            if(CurrentMode == Mode.Equipment)
            {
                switch (category)
                {
                    case Category.Weapon:
                        var inventory = TableHandler.Get("OwnedWeapons", TableHandler.StreamMode.AppData);
                        inventory.Add(current.GeneratedItem.Values.ToList());
                        inventory.Save();
                        break;
                    case Category.Hull:
                        var hullInventory = TableHandler.Get("OwnedHulls", TableHandler.StreamMode.AppData);
                        hullInventory.Add(current.GeneratedItem.Values.ToList());
                        hullInventory.Save();
                        break;
                    case Category.Utilities:
                        break;
                }

                current.SetSold();
                state = State.None;

                ConstructionController.Instance.UI.Conbay.SlotSelect.ItemController.ClearItems();
                ConstructionController.Instance.UI.Conbay.ShipSelect.ClearItems();
            }
            else
            {
                // 이 내용은 밑의 SetSold에 들어가야 함, 블루프린트 추가는 잠시 기능 정지

                switch (category)
                {
                    case Category.Weapon:
                        var weaponOwnedBluePrint = TableHandler.Get("OwnedBluePrintWeapons", TableHandler.StreamMode.AppData);
                        List<string> weaponBluePrintInfo = new List<string>
                        {
                            "0",
                            current.Row.Get<string>("index")
                        };
                        weaponOwnedBluePrint.Add(weaponBluePrintInfo);
                        weaponOwnedBluePrint.Save(TableHandler.StreamMode.AppData);
                        break;
                    case Category.Hull:
                        var hullOwnedBluePrint = TableHandler.Get("OwnedBluePrintHulls", TableHandler.StreamMode.AppData);
                        List<string> hullBluePrintInfo = new List<string>
                        {
                            "0",
                            current.Row.Get<string>("index")
                        };
                        hullOwnedBluePrint.Add(hullBluePrintInfo);
                        hullOwnedBluePrint.Save(TableHandler.StreamMode.AppData);
                        break;
                    case Category.Utilities:
                        break;
                }

                uiStoreView.UpdateUI();
                current.SetSold();
            }

        }

        // Callback fucn for unity button.
        public void ChangeStoreCategory(string category)
        {
            Category newCategory = (Category)Enum.Parse(typeof(Category), category, true);

            if (newCategory == this.category)
                return;

            uiStoreView.ActiveCategoryItems(newCategory);
            current = null;
            state = State.None;
            this.category = newCategory;
        }

        public Mode SwapMode()
        {
            if(mode == Mode.Equipment)
            {
                ChangeStoreMode("BluePrint");
            }
            else
            {
                ChangeStoreMode("Equipment");
            }

            return mode;
        }


        public void ChangeStoreMode(string mode)
        {
            Mode newMode = (Mode)Enum.Parse(typeof(Mode), mode, true);

            if (newMode == this.mode)
                return;

            uiStoreView.ChangeMode(newMode);
            current = null;
            state = State.None;
            this.mode = newMode;
        }
    }
}


