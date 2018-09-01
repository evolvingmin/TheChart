using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReturnToEarth
{
    public class UnitManager : MonoBehaviour
    {
        private Dictionary<string, GameObject> loadedPrefabs;

        private Vector3 unitScale;

        private BoardManager boardController;
        private ResourceManager resourceManager;

        public GameDefine.Result Initialize(BoardManager _boardController, ResourceManager resourceManager, Vector3 uniformCenter, Vector3 uniformScale)
        {
            //unitCenter = uniformCenter;
            unitScale = uniformScale;
            boardController = _boardController;
            this.resourceManager = resourceManager;
            

            return GameDefine.Result.OK;
        }

        public GameDefine.Result GenerateUnit(string Type, int x, int y, GameManager.Team team)
        {
            Block block = boardController.GetBlock(x, y);
            GameObject unitObject = resourceManager.GetObject<GameObject>("Unit", Type);
            unitObject.transform.SetParent(transform);
            Unit unit = unitObject.GetComponent<Unit>();
            unit.Initialize(unitScale, block, team);

            return GameDefine.Result.OK;
        }
    }

}
