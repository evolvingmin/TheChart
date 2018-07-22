using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReturnToEarth
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField]
        private int width = 5;

        [SerializeField]
        private int height = 5;

        [SerializeField]
        private Vector3 center;

        [SerializeField]
        private Vector3 blockScale;

        [SerializeField]
        private GameObject blockPrefab;

        private List<List<Block>> blocks;

        public void OnEnable()
        {
            Debug.Log(Initialize());
        }

        public GameDefine.Result Initialize()
        {
            if (width <= 0 || height <= 0)
                return GameDefine.Result.ERROR_DATA_NOT_IN_PROPER_RANGE;

            float startPosX = ( center.x - ( ( ( blockScale.x * width ) / 2.0f ) - ( blockScale.x ) / 2.0f ) );
            float startPosY = ( center.y + ( ( ( blockScale.y * height ) / 2.0f ) - ( blockScale.y ) / 2.0f ) );

            float currentPosX = startPosX;
            float currentPosY = startPosY;

            blockPrefab.SetActive(true);
            blocks = new List<List<Block>>();
            for (int i = 0; i < height; i++)
            {
                blocks.Add(new List<Block>());
                for (int j = 0; j < width; j++)
                {
                    GameObject created = Instantiate(blockPrefab, new Vector3(currentPosX, currentPosY, center.z), Quaternion.identity, transform);
                    Block currentBlock = created.GetComponent<Block>();
                    currentBlock.Initialize(new Vector2(i, j), created.transform.position);
                    currentPosX += blockScale.x;
                    currentBlock.transform.localScale = blockScale;
                    currentBlock.name = "Block(" + i + "," + j + ")";
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

        // Update is called once per frame
        void Update()
        {

        }
    }

}
