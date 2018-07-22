using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace StarShip {
    public class GameManager : MonoBehaviour {
        public static int stageNumber = 0;

        // Singleton Implementation.
        private static GameManager instance = null;
        public static GameManager Instance {
            get {
                if ( instance == null )
                    instance = (GameManager)FindObjectOfType(typeof(GameManager));
                return instance;
            }
        }

        [SerializeField]
        private Board board;
        public Board Board { get { return board; } }

        [SerializeField]
        private EnemyGenerator enemyGenerator;
        public EnemyGenerator EnemyGenerator { get { return enemyGenerator; } }

        [SerializeField]
        private Transform startPoint;

        [SerializeField]
        private BulletController bulletController;
        public BulletController BulletController { get { return bulletController; } }

        [SerializeField]
        private DeckManager deckManager;

        
        //private WealthManager wealthManager;
        
        //[SerializeField]
        //private Text wealthDisplayer;

        private StarShip starShip = null;
        //[SerializeField]
        public StarShip StarShip {  get { return starShip; } }

        public GameInfo Game {  get { return game; } }
        private GameInfo game;

        private void Start()
        {
            bool result = true;

            game = new GameInfo();

            StarShip starship = null;
            StarShip.GenerateStarShip(StarShip.Mode.Battle, out starship, 0);
            starship.transform.position = startPoint.position;
            starship.transform.localScale = Vector3.one * 0.54f;
            this.starShip = starship;
            result &= board.Initialize();
            result &= enemyGenerator.Initialize(board, starship, stageNumber);
            result &= bulletController.Initialize();
            result &= deckManager.Initialize(starShip.GetCardList());
        }

    }

}

