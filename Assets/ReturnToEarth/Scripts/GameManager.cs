using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReturnToEarth
{
    // Im pretty sure I have focus issue to keep fail to continue project for helping myself. 
    // by making public project and i hope this will be the daily, or at least weekly task for me to keep motivating to make something just seeting in the home alone do nothing. so has to be this is my motivation box.

    // At least, I will be chatty in this small open place will nobody visited in here.

    // and At least, Hope someday person like me also can make some stady ground for jumping for the next step, like getting happiness or making place for feels alive. 

    // I hope so...

    // 개략적으로 어떤 게임이 될 것인가에 대해서 다시 또 상상을 해야 하지만, 퍼블릭으로 돌린만큼 해비하게 뭐 
    // 새로운 시도를 하겠습니다가 아니라, 그냥 내가 생각하기에 재미있을거 같습니다 와 같이 즉흥적으로 구현하자
    // 아직 걸음마도 제대로 못땐 XX가 너무 비싸게 굴지 말자. 아기가 걸음마도 못배운 상태인데 어떻게 하면 완벽한
    // 마라톤을 할 수 있을까 라는 고민을 머리속에 들고 있으니 시작도 못하는거다. 걸음마부터 좀 해보지 않을래?

    // 그리고 좀 집착좀 해줘봐... 좀...

    public class GameManager : MonoBehaviour
    {
        public enum Team
        {
            None,
            Player,
            Opponent
        }

        [SerializeField]
        private Vector3 uniformScale;
        [SerializeField]
        private Vector3 uniformCenter;

        // Use this for initialization

        [SerializeField]
        private BoardManager boardManager;

        [SerializeField]
        private UnitManager unitManager;

        [SerializeField]
        private ResourceManager resourceManager;
        public ResourceManager ResourceManager
        {
            get { return resourceManager; }
        }

        [SerializeField]
        private BulletManager bulletManager;
        public BulletManager BulletManager
        {
            get { return bulletManager; }
        }

        // Singleton Implementation.
        private static GameManager instance = null;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                    instance = (GameManager)FindObjectOfType(typeof(GameManager));
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
            Define.Result results;
            results = resourceManager.Initialize("ReturnToEarth");
            results = bulletManager.Initialize(resourceManager);
            results = boardManager.Initialize(resourceManager, uniformCenter, UniformScale);
            results = unitManager.Initialize(boardManager, resourceManager, uniformCenter, UniformScale);

            // 개략적인 게임 흐름을 그린다면.
            // 레벨디자인 제어는 게임 컨트롤러에서 한다.
            // 레벨 디자인에 맞는 데이터를 가져오고
            // 해당 데이터를 엑터 컨트롤러에서 생성하도록 한다.
            // 레벨 디자인이 있는 셈 치고 테스트 코드 형식으로 넣도록 하자.

            unitManager.GenerateUnit("Base", "BaseEnemy", 0, 0, Team.Opponent);
            unitManager.GenerateUnit("Base", "BaseEnemy", 1, 1, Team.Opponent);
            unitManager.GenerateUnit("Base", "BaseEnemy", 2, 2, Team.Opponent);
            unitManager.GenerateUnit("Base", "BaseEnemy", 3, 2, Team.Opponent);
            unitManager.GenerateUnit("Base", "BaseEnemy", 2, 4, Team.Opponent);

            unitManager.GenerateUnit("Base", "BasePlayer", 3, 7, Team.Player);
            unitManager.GenerateUnit("Base", "BasePlayer", 4, 7, Team.Player);

            Debug.Log("GameController Initialized, Result is : " + results);
        }

    }
}
