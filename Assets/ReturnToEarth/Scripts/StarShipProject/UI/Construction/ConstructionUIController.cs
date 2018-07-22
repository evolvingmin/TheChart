using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarShip.UI
{
    public class ConstructionUIController : MonoBehaviour
    {
        public enum Mode
        {
            None,
            BattleShip,
            Conbay,
            Store,
            StageSelect
        }

        [SerializeField]
        private GameObject stageSelectPanel = null;

        [SerializeField]
        private GameObject battleShipPanel = null;

        [SerializeField]
        private UIStore equipmentPanel = null;
        public UIStore Equipment {  get { return equipmentPanel; } }

        [SerializeField]
        private UIConbay conbayPanel = null;
        public UIConbay Conbay {  get { return conbayPanel; } }
        
        // private ConstructionController constructionController = null;
        private Mode mode = Mode.BattleShip;
        public Mode ConstructionMode {  get { return mode; } }

        [SerializeField]
        private Image gradationImage;

        [SerializeField]
        private float fadeTime = 1.0f;

        [SerializeField]
        private float minfadeAlphaValue = 0.5f;

        public bool Initialze(ConstructionController constructionController)
        {
           // this.constructionController = constructionController;
            stageSelectPanel.SetActive(false);
            battleShipPanel.SetActive(true);
            Conbay.Initialze(this);
            Equipment.Initialize(this);
            GetherPanel();

            return true;
        }

        public void ActivePanel(string panelName)
        {
            Mode nextMode = (Mode)Enum.Parse(typeof(Mode), panelName, true);

            if (nextMode == mode)
                return;
            mode = nextMode;

            
            switch (nextMode)
            {
                case Mode.BattleShip:

                    stageSelectPanel.SetActive(false);
                    battleShipPanel.SetActive(true);
                    Equipment.Active(false);
                    conbayPanel.Active(false);
                    gradationImage.CrossFadeAlpha(1.0f, fadeTime, false);
                    break;
                case Mode.Conbay:
                    stageSelectPanel.SetActive(false);
                    battleShipPanel.SetActive(false);
                    Equipment.Active(false);
                    conbayPanel.Active(true);
                    gradationImage.CrossFadeAlpha(minfadeAlphaValue, fadeTime, false);
                    break;
                case Mode.Store:
                    stageSelectPanel.SetActive(false);
                    battleShipPanel.SetActive(false);
                    Equipment.Active(true);
                    conbayPanel.Active(false);
                    gradationImage.CrossFadeAlpha(1.0f, fadeTime, false);
                    break;
                case Mode.StageSelect:
                    stageSelectPanel.SetActive(true);
                    battleShipPanel.SetActive(false);
                    Equipment.Active(false);
                    conbayPanel.Active(false);
                    gradationImage.CrossFadeAlpha(minfadeAlphaValue, fadeTime, false);
                    break;
            }
        }

        public void ActiveStageSelect()
        {
            stageSelectPanel.SetActive(true);
            battleShipPanel.SetActive(false);
            conbayPanel.gameObject.SetActive(false);
        }

        public void GetherPanel()
        {
            gradationImage.transform.Reset();
            stageSelectPanel.transform.Reset();
            battleShipPanel.transform.Reset();
            equipmentPanel.transform.Reset();
            conbayPanel.transform.Reset();
        }
    }

}
