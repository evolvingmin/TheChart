using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReturnToEarth
{
    public class ActorController : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] enemySprites;

        [SerializeField]
        private Sprite[] fiendlySprites;

        private Vector3 unitCenter;
        private Vector3 unitScale;

        [SerializeField]
        private GameObject baseUnitPrefab;

        private BoardController boardController;

        public GameDefine.Result Initialize(BoardController _boardController, Vector3 uniformCenter, Vector3 uniformScale)
        {
            unitCenter = uniformCenter;
            unitScale = uniformScale;
            boardController = _boardController;

            return GameDefine.Result.OK;
        }

        // Need Implementation ObjectPool.

        public GameDefine.Result GenerateUnit(int x, int y, Unit.Team team)
        {
            bool rotated = team == Unit.Team.Enemy;

            baseUnitPrefab.SetActive(true);

            int selectedSpriteIndex = Random.Range(0, 5);

            Sprite selectedSprite = team == Unit.Team.Friendly ? fiendlySprites[selectedSpriteIndex] : enemySprites[selectedSpriteIndex];

            Block block = boardController.GetBlock(x, y);
            GameObject unitObject = Instantiate(baseUnitPrefab, block.transform.position, Quaternion.identity, transform);
            Unit unit = unitObject.GetComponent<Unit>();
            unit.Initialize(unitScale, selectedSprite, block, team);

            baseUnitPrefab.SetActive(false);
            return GameDefine.Result.OK;
        }
    }

}
