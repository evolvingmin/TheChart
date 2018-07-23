using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace StarShip
{
    // 보드 데이터를 정의하고
    // 정의된 데이터를 바탕으로 맵 상에 그려야 한다.
    [ExecuteInEditMode]
    public class Board : MonoBehaviour
    {
        // 라인 하나를 기준으로 데이터를 정하기 위함.
        public class LineData
        {
            public int availableCellCount;
            public bool IsLineAvailable
            {
                get
                {
                    return availableCellCount > 0;
                }
            }
        }

        public enum LineType
        {
            Far,
            Middle,
            Near,
        }

        [SerializeField]
        private float boardYRatio = 0.7f;

        private int maxLine = 3;
        private int maxRow = 3;
        private int maxColumn = 7;


        private Cell[,,] cells;
        private LineData[] lineData;

        // We will use this as BoardSize (need advise what actually cause bring bad result..)
        [SerializeField]
        private BoxCollider2D boardArea = null;

        private float boaderLength = 0.1f;

        Vector3 leftTop;
        Vector3 rightTop;
        Vector3 leftBottom;
        Vector3 rightBottom;

        private Vector3 leftTopCellStart;

        private float sizeX;
        private float sizeY;

        private float cellSizeX;
        private float cellSizeY;

        private int centerStartIndex = 1;
        private int centerEndIndex = 4;

        private bool isInitialized = false;

        
        public int TotalCellLength {  get { return cells.Length; } }
        
        public BoardIndicator BoardIndicator {  get { return boardIndicator; } }
        private BoardIndicator boardIndicator;

        private void Awake()
        {
            boardIndicator = GetComponent<BoardIndicator>();
        }

        public bool Initialize()
        {
            // Define Array
            isInitialized = false;
            cells = new Cell[maxLine, maxRow, maxColumn];
            lineData = new LineData[maxLine];

            sizeX = boardArea.size.x;
            sizeY = boardArea.size.y * boardYRatio;

            cellSizeX = ( sizeX - boaderLength * 6.0f ) / maxColumn;
            cellSizeY = ( sizeY - boaderLength * 6.0f ) / ( maxRow * maxLine );

            // Calculate Position and Cell Sized
            rightTop = boardArea.bounds.max;
            rightTop.z = 0.0f;
            leftBottom = new Vector3(boardArea.bounds.min.x, boardArea.bounds.max.y - sizeY, 0.0f);
            leftTop = new Vector3(leftBottom.x, rightTop.y, 0.0f);
            rightBottom = new Vector3(rightTop.x, leftBottom.y, 0.0f);

            GenerateBoard();
            boardIndicator.GenerateIndicators(this ,maxLine, maxRow, maxColumn, cells); 
            
            return true;
        }

        private void GenerateBoard()
        {
            Vector3 currentHandle = GetNextPos(leftTop, boaderLength, boaderLength);
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                currentHandle = GenerateLine(cells, i, currentHandle);
                currentHandle = GetNextPos(currentHandle, 0, boaderLength*2);
            }
            Debug.Log("CellGenerated");
            isInitialized = true;
        }

        private Vector3 GenerateLine(Cell[,,] lineArray, int lineIndex, Vector3 current)
        {
            lineData[lineIndex] = new LineData()
            {
                availableCellCount = lineArray.GetLength(1) * lineArray.GetLength(2)
            };

            for (int i = 0; i < lineArray.GetLength(1); i++)
            {
                for (int j = 0; j < lineArray.GetLength(2); j++)
                {
                    lineArray[lineIndex, i, j] = new Cell();
                    current = lineArray[lineIndex, i, j].Initialize(current, cellSizeX, cellSizeY, new Vector3(lineIndex, i, j));
                    if (j == centerStartIndex || j == centerEndIndex)
                    {
                        current = GetNextPos(current, boaderLength * 2, 0);
                    }
                }
                current = GetNextPos(current, ( -sizeX + boaderLength * 2 ), cellSizeY);
            }
            return current;
        }


        private void OnDrawGizmosSelected()
        {
            if (!isInitialized)
                return;
            // Draw Board
            DrawGizmos.DrawRect2D(leftTop, sizeX, sizeY, Color.blue);
            DrowGizmoBlock(0, Color.red);
            DrowGizmoBlock(1, Color.yellow);
            DrowGizmoBlock(2, Color.cyan);
        }

        private Vector3 GetNextPos(Vector3 current, float x, float y)
        {
            current.x += x;
            current.y -= y;
            return current;
        }

        public void DrowGizmoBlock(int lineIndex, Color color)
        {
            Gizmos.color = color;
            for (int i = 0; i < cells.GetLength(1); i++)
            {
                for (int j = 0; j < cells.GetLength(2); j++)
                {
                    Cell cell = cells[lineIndex, i, j];

                    Gizmos.DrawLine(cell.LeftTop, cell.RightTop);
                    Gizmos.DrawLine(cell.RightTop, cell.RightBottom);
                    Gizmos.DrawLine(cell.RightBottom, cell.LeftBotom);
                    Gizmos.DrawLine(cell.LeftBotom, cell.LeftTop);
                }
            }
        }

        public Cell GetAvailableCell(Defines.EnemyType enemyType, LineType lineType)
        {
            switch (enemyType)
            {
                case Defines.EnemyType.Minion:
                    return GetAvailableCellRandomly(lineType);
                case Defines.EnemyType.MiddleBoss:
                case Defines.EnemyType.Boss:
                    return GetBossCell();
            }
            return null;
        }

        public Cell GetBossCell()
        {
            for (int i = 0; i < cells.GetLength(1); i++)
            {
                for (int j = 2; j < 5; j++)
                {
                    if(cells[1,i,j].Occupied)
                    {
                        Enemy current = cells[1, i, j].Release();
                        Cell targetCell = GetAvailableCellRandomly((LineType)(UnityEngine.Random.Range(0, 2) == 1 ? 0 : 2));
                        //Debug.Log("Wrong Dest??????" + targetCell.Index);
                        current.Launch(current.transform.position, targetCell);
                    }
                }

            }
 
            return cells[1, 1, 3];
        }


        // 개선 방향 : 이전에 넣었던 기록들을 스텍으로 구성하여, 그 리스트를 점검한 뒤에 모두 차 있을 경우에는 함수가 발동하지 않도록 해야 함.
        // 나중에 깔린 몬스터들이 제거 되면, 그 리스트 역시 갱신하여야 한다.

        // 무슨 정보가 필요한가?
        // 라인마다 남아있는 수 만큼을 체크하여, 그 사이 내에서 랜덤으로 돌려야 한다.
        // 관련 있을 수 있는 정보를 모두 나열 해 보자
        // 남아 있는 셀의 수
        // 현재 까지 방문한 셀의 리스트
        // 렌덤으로 찾는데 target.Occupied를 일일이 체크하면서 삽질 시키는게 연산상 효율이 좋아 보이지 않는다.
        // 

        public void IncreaseAvailableLine(LineType linetype)
        {
            lineData[(int)linetype].availableCellCount++;
        }


        public Cell GetAvailableCellRandomly( LineType lineType)
        {
            //if (preveLine == 0 && preveRow == 0 && preveColumn == 0) // 최초 처음이라는 의미.
            int newRow = UnityEngine.Random.Range(0, maxRow);
            int newColumn = UnityEngine.Random.Range(0, maxColumn);
            int newLine = (int)lineType;

            Cell target = cells[newLine, newRow, newColumn];

            if(target.Occupied)
            {
                if(lineData[newLine].IsLineAvailable)
                {
                    return GetAvailableCellRandomly(lineType);
                }
                else
                {
                    for (int i = 0; i < maxLine; i++)
                    {
                        if (i != newLine)
                        {
                            if (lineData[i].IsLineAvailable)
                            {
                                return GetAvailableCellRandomly( (LineType)i );
                            }
                        }
                    }

                    return null;
                } 
            }
            
            lineData[newLine].availableCellCount--;
            //Debug.LogFormat("Current Remaining Set, CurrentLine : {0}, availableCellCount {1}", newLine, lineData[newLine].availableCellCount);
            return target;
        }

    }

}
