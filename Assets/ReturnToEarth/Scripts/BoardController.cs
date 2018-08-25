using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReturnToEarth
{
    public class BoardController : MonoBehaviour
    {
        // https://stackoverflow.com/questions/55984/what-is-the-difference-between-const-and-readonly
        [SerializeField]
        private readonly int width = 5;

        [SerializeField]
        private readonly int height = 5;

        private Vector3 blockCenter;
        private Vector3 blockScale;

        [SerializeField]
        private GameObject blockPrefab;

        private List<List<Block>> blocks;

        //private bool isInitializedInEditor = false;

        public GameDefine.Result Initialize(Vector3 uniformCenter, Vector3 uniformScale)
        {
            if (width <= 0 || height <= 0)
                return GameDefine.Result.ERROR_DATA_NOT_IN_PROPER_RANGE;

            blockCenter = uniformCenter;
            blockScale = uniformScale;

            float startPosX = ( blockCenter.x - ( ( ( blockScale.x * width ) / 2.0f ) - ( blockScale.x ) / 2.0f ) );
            float startPosY = ( blockCenter.y + ( ( ( blockScale.y * height ) / 2.0f ) - ( blockScale.y ) / 2.0f ) );

            float currentPosX = startPosX;
            float currentPosY = startPosY;

            blockPrefab.SetActive(true);
            blocks = new List<List<Block>>();
            for (int i = 0; i < height; i++)
            {
                blocks.Add(new List<Block>());
                for (int j = 0; j < width; j++)
                {
                    GameObject created = Instantiate(
                        blockPrefab, 
                        new Vector3(currentPosX, currentPosY, blockCenter.z), 
                        Quaternion.identity, transform);

                    Block currentBlock = created.GetComponent<Block>();
                    currentBlock.Initialize(new Vector2(i, j), created.transform.position, blockScale);
                    currentPosX += blockScale.x;

                    blocks[i].Add(currentBlock);
                }
                currentPosY -= blockScale.y;
                currentPosX = startPosX;
            }
            blockPrefab.SetActive(false);

            return GameDefine.Result.OK;
        }

        public GameDefine.Result IsInRange(int x, int y)
        {
            bool condition = ( x < width && y < height ) && ( x >= 0 && y >= 0 );

            if (condition == false)
                return GameDefine.Result.ERROR_DATA_NOT_IN_PROPER_RANGE;

            return GameDefine.Result.OK;
        }

        public Block GetBlock(int x, int y)
        {
            if(IsInRange(x,y) == GameDefine.Result.OK)
            {
                return blocks[x][y];
            }

            return null;
        }

        private void OnDrawGizmosSelected()
        {
            blockCenter = GameController.Instance.UniformCenter;
            blockScale = GameController.Instance.UniformScale;

            float startPosX = ( blockCenter.x - (  ( blockScale.x * width ) / 2.0f ));
            float startPosY = ( blockCenter.y + (  ( blockScale.y * height ) / 2.0f ));

            float currentPosX = startPosX;
            float currentPosY = startPosY;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    DrawGizmos.DrawRect2D( new Vector3(currentPosX, currentPosY), blockScale.x, blockScale.y, Color.blue);
                    currentPosX += blockScale.x;
                }

                currentPosY -= blockScale.y;
                currentPosX = startPosX;
            }
        }
    }

}
