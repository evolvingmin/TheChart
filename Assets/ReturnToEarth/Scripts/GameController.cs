using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReturnToEarth
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private Vector3 uniformScale;
        [SerializeField]
        private Vector3 uniformCenter;

        // Use this for initialization

        [SerializeField]
        private BoardController boardController;

        [SerializeField]
        private ActorController actorController;

        // Singleton Implementation.
        private static GameController instance = null;
        public static GameController Instance
        {
            get
            {
                if (instance == null)
                    instance = (GameController)FindObjectOfType(typeof(GameController));
                return instance;
            }
        }

        public Vector3 UniformScale
        {
            get
            {
                return uniformScale;
            }
        }

        public Vector3 UniformCenter
        {
            get
            {
                return uniformCenter;
            }
        }

        private void Awake()
        {
            GameDefine.Result results;

            results = boardController.Initialize(uniformCenter, UniformScale);
            results = actorController.Initialize(boardController, uniformCenter, UniformScale);

            // 개략적인 게임 흐름을 그린다면.

            // 레벨디자인 제어는 게임 컨트롤러에서 한다.
            // 레벨 디자인에 맞는 데이터를 가져오고
            // 해당 데이터를 엑터 컨트롤러에서 생성하도록 한다.
            // 레벨 디자인이 있는 셈 치고

            actorController.GenerateUnit(0, 0, Unit.Team.Enemy);
            actorController.GenerateUnit(1, 1, Unit.Team.Enemy);
            actorController.GenerateUnit(2, 2, Unit.Team.Enemy);
            actorController.GenerateUnit(3, 2, Unit.Team.Enemy);
            actorController.GenerateUnit(2, 4, Unit.Team.Enemy);

            actorController.GenerateUnit(2, 1, Unit.Team.Friendly);
            actorController.GenerateUnit(3, 4, Unit.Team.Friendly);
            actorController.GenerateUnit(4, 2, Unit.Team.Friendly);
            actorController.GenerateUnit(5, 4, Unit.Team.Friendly);
            actorController.GenerateUnit(4, 0, Unit.Team.Friendly);

            Debug.Log(results);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
