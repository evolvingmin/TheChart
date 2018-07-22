using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarShip.UI
{
    public class UIConbay : MonoBehaviour
    {
        public enum Mode
        {
            None,
            Main,
            ShipSelect,
            SlotSelect
        }

        [SerializeField]
        private UIConbaySlotSelect slotSelect = null;
        public UIConbaySlotSelect SlotSelect {  get { return slotSelect; } }

        [SerializeField]
        private UIShipSelect shipSelect = null;
        public UIShipSelect ShipSelect { get { return shipSelect; } }

        [SerializeField]
        private GameObject uiMain = null;

        private ConstructionUIController constructionUIController  = null;

        private Mode mode = Mode.Main;

        [SerializeField]
        private Transform commonPartRoot;

        public bool Initialze(ConstructionUIController constructionUIController)
        {
            this.constructionUIController = constructionUIController;
            mode = Mode.None;
            slotSelect.Initialze(this);
            uiMain.SetActive(false);
            shipSelect.Initialize(this);

            uiMain.transform.Reset();
            shipSelect.transform.Reset();
            slotSelect.transform.Reset();
            return true;
        }

        public void Active(bool value)
        {
            if(value)
            {
                uiMain.SetActive(true);
                commonPartRoot.gameObject.SetActive(true);
            }
            else
            {
                commonPartRoot.gameObject.SetActive(false);
                uiMain.SetActive(false);
                if(mode == Mode.Main)
                {
                    constructionUIController.ActivePanel("BattleShip");
                    mode = Mode.None;
                }
            }
        }

        public void ChangeMode_SlotChange(WeaponSlot weaponSlot)
        {
            uiMain.SetActive(false);
            slotSelect.Activate(weaponSlot);
            mode = Mode.SlotSelect;
        }

        public void ChangeMode_ShipSelect()
        {
            uiMain.SetActive(false);
            shipSelect.Active(true);
            mode = Mode.ShipSelect;
        }

        public void BackToMain()
        {
            uiMain.SetActive(true);
            mode = Mode.Main;
        }
    }

}

