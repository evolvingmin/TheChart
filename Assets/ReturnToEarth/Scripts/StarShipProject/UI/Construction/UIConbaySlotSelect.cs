using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarShip.UI
{
    public class UIConbaySlotSelect : MonoBehaviour
    {
        // 기본은 무기로 가정.
        public enum State
        {
            OFF,
            PROCESS_TO_ON,
            ON,
            ITEM_SELECTED
        }

        [SerializeField]
        private GameObject root = null;

        [SerializeField]
        private RectTransform weaponViewer = null;

        [SerializeField]
        private Camera conbayCamera = null;

        [SerializeField]
        private WeaponItemController weaponItemController = null;
        public WeaponItemController ItemController {  get { return weaponItemController; } }

        [SerializeField]
        private WeaponStatDisplayer weaponStatDisplayer = null;
        /*
        [SerializeField]
        private TotalStatDisplayer totalStatDisplayer = null;
        */
        [SerializeField]
        private Button selectionConfirm = null;

        private float approachSpeed = 4.0f;

        private float cameraPositionZ = 0.0f;
        private Vector3 origineCameraPosition;
        private State state = State.OFF;

        // Selected Information
        private WeaponSlot selectedSlot = null;
        private WeaponItem selectedweaponUI = null;
        private TableHandler.Row selectedWeaponRow = null;

        private UIConbay uiconbay = null;

        public bool Initialze(UIConbay uiconbay)
        {
            this.uiconbay = uiconbay;
            root.SetActive(false);
            selectionConfirm.interactable = false;
            bool InitializeFlag = weaponItemController.Initialize();
            cameraPositionZ = conbayCamera.transform.position.z;
            origineCameraPosition = conbayCamera.transform.position;
            return InitializeFlag;
        }

        public void UpdateSelectedItem(TableHandler.Row Info, WeaponItem selected)
        {
            if (selectedweaponUI != null && selectedweaponUI == selected)
                return;

            selectedweaponUI = selected;
            selectedWeaponRow = Info;
            weaponStatDisplayer.UpdateDisplayInfo(Info);
            weaponItemController.UpdateSelectInfo(selected);
            selectedSlot.UpdateSlot(selectedWeaponRow);
            state = State.ITEM_SELECTED;

            selectionConfirm.interactable = true;
        }

        public void OnConfirmClicked()
        {
            state = State.ON;
            selectionConfirm.interactable = false;
            selectedSlot.UpdateSlot(selectedWeaponRow);

            selectedWeaponRow = null;
        }

        public void Activate(WeaponSlot selected)
        {
            if (state != State.OFF)
                return;
            Vector3 displayerPosition = conbayCamera.ScreenToWorldPoint(weaponViewer.position);
            Vector3 TargetPosition = new Vector3(selected.transform.position.x - displayerPosition.x, selected.transform.position.y - displayerPosition.y, cameraPositionZ);
            selectedSlot = selected;
            weaponItemController.Initialize();
            weaponItemController.UpdateSelectInfo(selected.Row);
            StartCoroutine(MoveToTargetPosition(TargetPosition, State.ON));
            
        }

        public void ReturnToIDLE()
        {
            selectedSlot = null;
            selectedWeaponRow = null;
            selectionConfirm.interactable = false;
            StartCoroutine(MoveToTargetPosition(origineCameraPosition, State.OFF));
            
        }

        IEnumerator MoveToTargetPosition(Vector3 targetPosition, State destState)
        {
            state = State.PROCESS_TO_ON;
            root.SetActive(false);
            while (Vector3.Distance(conbayCamera.transform.position, targetPosition) > 0.1f)
            {
                conbayCamera.transform.position = Vector3.MoveTowards(conbayCamera.transform.position, targetPosition, approachSpeed * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }
            state = destState;

            switch (destState)
            {
                case State.OFF:
                    root.SetActive(false);
                    uiconbay.BackToMain();
                    break;
                case State.ON:
                    root.SetActive(true);
                    break;
            }
            
        }
    }

}
