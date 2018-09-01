using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarShip.UI
{
    public class UIStoreItem : MonoBehaviour
    {
        public enum Mode
        {
            Display,
            Selected,
            Sold
        }

        [SerializeField]
        private Text label = null;
        [SerializeField]
        private Button button = null;
        [SerializeField]
        private Image selected = null;
        [SerializeField]
        private Image icon = null;
        [SerializeField]
        private Image sold = null;

        public TableHandler.Row Row { get { return row; } }
        private TableHandler.Row row;

        private Dictionary<string,string> generatedItem;
        public Dictionary<string,string> GeneratedItem {  get { return generatedItem; } }


        private UIStore uiStore = null;

        public Mode ItemMode { get { return mode; } }
        private Mode mode = Mode.Display;

        public UIStore.Category Category { get { return category; } }
        private UIStore.Category category;

        public UIStore.Mode StoreMode { get { return storeMode; } }
        private UIStore.Mode storeMode;

        public bool Initialize(UIStore uiStore, TableHandler.Row row, UIStore.Category category, UIStore.Mode mode)
        {
            storeMode = mode;
            this.uiStore = uiStore;

            if (mode == UIStore.Mode.Equipment)
            {
                generatedItem = UIStoreView.GenerateItemByBluePrint(row, category);
            }
            else
            {
                generatedItem = null;
            }

            this.row = row;
            label.text = this.row.Get<string>("name");
            button.onClick.AddListener(ItemSelected);
            this.category = category;
            switch (category)
            {
                case UIStore.Category.Weapon:
                    icon.sprite = Resources.Load<Sprite>(string.Format("Image/Slot/Weapon/{0}/{1}", this.row.Get<string>("weapontype"), this.row.Get<string>("icon")));
                    break;
                case UIStore.Category.Hull:
                    icon.sprite = Resources.Load<Sprite>(string.Format("Image/Ship/{0}", this.row.Get<string>("icon")));
                    break;
                case UIStore.Category.Utilities:
                    // todo
                    break;
            }
            gameObject.SetActive(true);
            return true;
        }

        public void ItemSelected()
        {
            uiStore.UpdateSelected(this);
        }

        public void SetSelected(bool isSelected)
        {
            if (mode == Mode.Sold)
                return;
            
            if(isSelected)
            {
                mode = Mode.Selected;
            }
            else
            {
                mode = Mode.Display;
            }

            selected.gameObject.SetActive(isSelected);
        }

        public void SetSold()
        {
            mode = Mode.Sold;
            selected.gameObject.SetActive(false);
            sold.gameObject.SetActive(true);
        }
    }

}
