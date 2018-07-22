using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarShip.UI
{
    public class UIStoreSwapButton : MonoBehaviour
    {
        [SerializeField]
        private Text label;

        [SerializeField]
        private Button button;

        [SerializeField]
        private UIStore uiStore;

        private void Awake()
        {
            button.onClick.AddListener(ChangeMode);
            label.text = "Current : " + uiStore.CurrentMode.ToString();
        }
        
        public void ChangeMode()
        {
            UIStore.Mode currentMode = uiStore.SwapMode();
            label.text = "Current : " + currentMode.ToString();
        }
    }

}
