using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    // 정보체로서 하나의 단위를 Cell, 외부에 표시되는 Indicator를 Block이라고 부르자.
    // 이 Block들을 Indicator로서 역할을 하게 만드는 이녀석을 BoardIndicator 부르도록 하자.
    // 이녀석 Component가 되어야 한다.
    public class BoardIndicator : MonoBehaviour
    {
        private delegate void BlockDelegate(Block targetBlock);

        private Board board;

        private Block[,,] blocks;

        private int line, row, column;
        private Cell[,,] cells;

        private Vector3 origin;

        [SerializeField]
        private GameObject blockPrefab;

        [SerializeField]
        private LineRenderer ToInputLineRenderer;

        private Dictionary<string, Detecter> detectors = new Dictionary<string, Detecter>();
        private Detecter currentDetector = null;

        // 나중에 터치 테스트 할 때 Input에 오버라이드 하여 PC/스마트폰 상태에 따른 인풋 포지션을 달리 주는 방식을 선택해야 함.
        public Vector3 InputPosition { get { return Camera.main.ScreenToWorldPoint(Input.mousePosition); } }

        public bool GenerateIndicators(Board board,  int line, int row, int column, Cell[,,] cells)
        {
            this.board = board;
            blocks = new Block[line, row, column];
            blockPrefab.SetActive(true);

            this.line = line;
            this.row = row;
            this.column = column;
            this.cells = cells;

            for (int i =0; i< line; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    for (int k = 0; k < column;k++)
                    {
                        blocks[i, j, k] = (GameObject.Instantiate(blockPrefab).GetComponent<Block>());
                        var CurrentBlock = blocks[i, j, k];
                        CurrentBlock.transform.position = cells[i, j, k].Center;
                        CurrentBlock.transform.SetParent(board.transform);
                        CurrentBlock.transform.localScale = new Vector2(cells[i, j, k].rect.width, cells[i, j, k].rect.height);
                        CurrentBlock.Initialize(cells[i, j, k]);
                    }
                }
            }

            blockPrefab.SetActive(false);

            origin = GameManager.Instance.StarShip.transform.position;
            return true;
        }

        public Detecter AddDetector(string detectorName)
        {
            if(!detectors.ContainsKey(detectorName))
            {
                Detecter loaded = Instantiate(Resources.Load<Detecter>(string.Format("Prefabs/Detector/{0}", detectorName)));
                detectors.Add(detectorName, loaded);
                loaded.transform.SetParent(transform);
                loaded.gameObject.SetActive(false);
                return loaded;
            }

            return GetDetectorByName(detectorName);
        }

        public void Update()
        {
            if (currentDetector == null)
                return;

            switch (currentDetector.type)
            {
                case Detecter.Type.ToInput:
                    // +90 ~ -90 선상의 z 값을 바꾸는게 의도.
                    float toAngle = MathEx.SignedAngle(Vector3.up, new Vector3(InputPosition.x, InputPosition.y,0) - origin, Vector3.forward);
                    currentDetector.transform.rotation = Quaternion.Euler(new Vector3(0, 0, toAngle));
                    // 로테이션을 넣어줘야 함.
                    break;
                case Detecter.Type.Input:
                    currentDetector.transform.position = InputPosition;
                    break;
            }
        }

        public Detecter GetDetectorByName(string name)
        {
            if(detectors.ContainsKey(name))
            {
                return detectors[name];
            }

            // error
            return null;
        }

        public void SetEnableDetect(CardInfo RequestedCardInfo, bool value)
        {
            currentDetector = GetDetectorByName(RequestedCardInfo.DetectorName);

            if(currentDetector == null)
            {
                Debug.Log("Detector Couldn't find..." + RequestedCardInfo.DetectorName);
                return;
            }

            currentDetector.gameObject.SetActive(value);

            if(value)
            {
                ActivateActionAll(ActiveBlock);
                switch (currentDetector.type)
                {
                    case Detecter.Type.Origin:
                        currentDetector.transform.position = origin;
                        break;
                    case Detecter.Type.ToInput:
                        currentDetector.transform.position = origin;
                        break;
                    case Detecter.Type.Input:
                        currentDetector.transform.position = InputPosition;
                        break;
                }
            }
            else
            {
                ActivateActionAll(DeactiveBlock);
            }
        }

        private void ActiveBlock(Block block)
        {
            block.SetState(Block.State.Detectable);
        }
        
        private void DeactiveBlock(Block block)
        {
            block.SetState(Block.State.None);
        }

        private void ActivateActionAll(BlockDelegate blockDelegate)
        {
            for (int i = 0; i < line; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    for (int k = 0; k < column; k++)
                    {
                        blockDelegate.Invoke(blocks[i, j, k]);
                    }
                }
            }
        }
    }
}
