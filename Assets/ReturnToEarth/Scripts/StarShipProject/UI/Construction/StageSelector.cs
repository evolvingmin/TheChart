using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip.UI
{
    public class StageSelector : MonoBehaviour
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private Transform root;

        private void Awake()
        {
            Initialize();
        }

        public bool Initialize()
        {
            var ItemList = StageInfoTable.GetRowList();
            foreach (var item in ItemList)
            {
                UIStageButton uiStageButton = Instantiate(prefab).GetComponent<UIStageButton>();
                uiStageButton.transform.SetParent(root);
                uiStageButton.transform.Reset();
                uiStageButton.Initialize(item.i_index);
            }
            prefab.SetActive(false);
            return true;
        }
    }
}