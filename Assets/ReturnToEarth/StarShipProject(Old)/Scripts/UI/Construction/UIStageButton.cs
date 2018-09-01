using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace StarShip.UI
{
    public class UIStageButton : MonoBehaviour
    {
        [SerializeField]
        private Text stageLabel;
        [SerializeField]
        private Button button;

        public bool Initialize(int stageNumber)
        {
            stageLabel.text = "Stage : " + stageNumber;
            GameManager.stageNumber = stageNumber;
            button.onClick.AddListener(ConstructionController.Instance.MoveToBattleScene);
            return true;
        }
    }
}