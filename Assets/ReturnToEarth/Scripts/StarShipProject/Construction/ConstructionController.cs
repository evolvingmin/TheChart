using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 해야 할 일들
// 전투선의 해당 부품을 선택 할 시 그곳을 UI의 중앙부분(은 정확히 아님) 으로 이동 시키고 UI를 활성화 시킴
// 이동할 위치는 정중앙이 아니라, UI컴포넌트의 WeaponDisplayer의 정중앙으로 이동해야 함
// 하기 위해서 UI 상의 정중앙과 weaponDisplayer상의 오프셋 값을 구하고, 이만큼 
// 뒤로가기 버튼을 누를 경우 다시 UI 켄버스를 비 활성화 시키고 정 중앙으로 감.
// 

// 테이블을 읽어 들여서 아이템 리스트들을 구성한다.

// 플레이어 함선에 대한 정보 구성이 필요하다.
// 


namespace StarShip
{
    public class ConstructionController : MonoBehaviour
    {
        [SerializeField]
        private UI.ConstructionUIController uiController = null;
        public UI.ConstructionUIController UI
        {
            get { return uiController; }
        }

        private StarShip currentStarShip = null;

        // Singleton Implementation.
        private static ConstructionController instance = null;
        public static ConstructionController Instance
        {
            get
            {
                if (instance == null)
                    instance = (ConstructionController)FindObjectOfType(typeof(ConstructionController));
                return instance;
            }
        }

        private void Awake()
        {

            uiController.Initialze(this);
            ChangeStarship();
        }

        public void ChangeStarship(int slotIndex = 0)
        {
            if(currentStarShip != null)
            {
                DestroyImmediate(currentStarShip.gameObject);
            }

            StarShip starship = null;
            StarShip.GenerateStarShip(StarShip.Mode.Construction, out starship, slotIndex);

            currentStarShip = starship;
        }

        public void UpdateStarShip(string hullIndex, int slotIndex = 0)
        {
            currentStarShip.UpdateSlotInfo(hullIndex, "", slotIndex);
            ChangeStarship(slotIndex);
        }

        public void MoveToBattleScene()
        {
            currentStarShip.SaveCurrentDataAsCSV(0);
            //SceneManager.LoadScene("BattleScene", LoadSceneMode.Single);
            SSSceneManager.Instance.MoveScene("BattleScene");
        }
        

        
    }

}
