using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    public class GameInfo
    {
        protected GameManager GM {  get { return GameManager.Instance; } }

        public List<BattleObject> GetAllEnemy()
        {
            return GM.EnemyGenerator.GetAllEnemy();
        }
        // 스타쉽에 접근하고 싶어요
        // 나 애하나 소환하고 싶어요

        public List<BattleObject> GetDetectedEnemy(string detectorName, BoardIndicator boardIndicator)
        {
            var currentDetector = boardIndicator.GetDetectorByName(detectorName);
            List<Cell> detectedCellList = currentDetector.GetDetectedCells();

            return GM.EnemyGenerator.GetAllEnemyByCells(detectedCellList);
        }
    }

    public class GameWealth {

        //static string csvFN = "Table/GameWealth";

        public static void Start(string path) {
                        
            List<Dictionary<string, object>> data = CSVReader.Read(path);
            /*
            for ( var i = 0; i < data.Count; i++ ) {
                Debug.Log(data[0][0]);
            }*/
            
        }



        
    }

}
