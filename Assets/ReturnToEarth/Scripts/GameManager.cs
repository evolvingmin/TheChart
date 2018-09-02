﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReturnToEarth
{
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

            unitManager.GenerateUnit("Base", "BasePlayer", 7, 3, Team.Player);
            unitManager.GenerateUnit("Base", "BasePlayer", 7, 4, Team.Player);

            Debug.Log("GameController Initialized, Result is : " + results);
        }

    }
}
