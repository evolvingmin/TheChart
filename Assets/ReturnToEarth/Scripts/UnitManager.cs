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

        public Define.Result Initialize(BoardManager _boardController, ResourceManager resourceManager, Vector3 uniformCenter, Vector3 uniformScale)
        {
            //unitCenter = uniformCenter;
            unitScale = uniformScale;
            boardController = _boardController;
            this.resourceManager = resourceManager;

            return Define.Result.OK;
        }

        public Define.Result GenerateUnit(string type, string data, int x, int y, GameManager.Team team)
        {
            Block block = boardController.GetBlock(x, y);
            GameObject unitObject = resourceManager.GetObject<GameObject>("Unit", type);
            unitObject.transform.SetParent(transform);
            Unit unit = unitObject.GetComponent<Unit>();
            unit.Initialize(data, unitScale, block, team);

            return Define.Result.OK;
        }
    }

}
