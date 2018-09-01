using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReturnToEarth
{
    public class ActorController : MonoBehaviour
    {
        private Dictionary<string, GameObject> loadedPrefabs;

        private Vector3 unitScale;

        private BoardController boardController;
        private ResourceManager resourceManager;

        public GameDefine.Result Initialize(BoardController _boardController, ResourceManager resourceManager, Vector3 uniformCenter, Vector3 uniformScale)
        {
            //unitCenter = uniformCenter;
            unitScale = uniformScale;
            boardController = _boardController;
            this.resourceManager = resourceManager;
            

            return GameDefine.Result.OK;
        }

        // Need Implementation ObjectPool.

        public GameDefine.Result GenerateUnit(string Type, int x, int y, Unit.Team team)
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
