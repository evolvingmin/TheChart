using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

namespace StarShip
{
    public class EnemyGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject enemyPrefab;
        [SerializeField]
        private Transform startingPoint;

        private StarShip player = null;
        private Board board;
        private Stack<Enemy> enemyPools = null;

        private int currentIndex = 0;
        private int totalPhase = 0;
        private int minionCountPerPhase = 0;
        private int currentPhase = 0;

        private StageInfoTable.Row currentStageInfo = null;
        private int smallMonsterCount = 0;
        private MonsterInfoTable.Row[] smallMonsterList = null;

        private MonsterInfoTable.Row[] middleBossList = null;
        private MonsterInfoTable.Row bossInfo = null;

        private Enemy currentBoss = null;
        private List<Enemy> currentEnemies = new List<Enemy>();
        private Dictionary<string, Sprite> loadedSprite = new Dictionary<string, Sprite>();


        public BattleObject StartshipObject { get { return starshipObject; } }
        private BattleObject starshipObject;


        IEnumerator ProgressStage(float LaunchDuration, int TotalMinionCount)
        {
            currentPhase = 0;
            int currentMinionCount = 0;
            while (currentMinionCount < TotalMinionCount)
            {
                // 중간 보스 몬스터 나올 타이밍
                if(currentMinionCount == (currentPhase + 1) * minionCountPerPhase)
                {
                    while(!CanLaunchAble())
                        yield return null;

                    currentBoss = Launch(Defines.EnemyType.MiddleBoss);

                    while (currentBoss.EnemyMode != Enemy.Mode.Dead)
                        yield return null;
                    Debug.Log("MonsterInfo Middle Boss Died! NextPhase : " + currentPhase);
                    currentBoss = null;
                    currentPhase++;
                    yield return new WaitForSeconds(LaunchDuration);
                }
                else
                {
                    if (Launch( Defines.EnemyType.Minion) != null)
                        currentMinionCount++;
                    yield return new WaitForSeconds(LaunchDuration);
                }
            }

            // 보스 몬스터

            while (!CanLaunchAble())
                yield return null;

            currentBoss = Launch(Defines.EnemyType.Boss);

            while (currentBoss.EnemyMode != Enemy.Mode.Dead)
                yield return null;

            Debug.Log("MonsterInfo Boss Died! Stage End");

            yield return new WaitForSeconds(3);

            Debug.Log("Move to ConstructionScene");

            SceneManager.LoadScene("Construction", LoadSceneMode.Single);

        }

        public bool Initialize(Board board, StarShip starship, int stageNumber)
        {
            this.board = board;
            player = starship;
            enemyPools = new Stack<Enemy>(board.TotalCellLength);
            for (int i = 0; i < board.TotalCellLength; i++)
            {
                Enemy enemy = Instantiate(enemyPrefab).GetComponent<Enemy>();
                enemyPools.Push(enemy);
                enemy.transform.SetParent(transform);
                enemy.gameObject.SetActive(false);
            }
            enemyPrefab.SetActive(false);

            currentStageInfo = StageInfoTable.GetAt(stageNumber - 1); 
            smallMonsterCount = currentStageInfo.i_totalSmallMonster;
            smallMonsterList = currentStageInfo.SmallMonsterList;

            middleBossList = currentStageInfo.MiddleBossList;
            bossInfo = currentStageInfo.BossList;
            totalPhase = middleBossList.Length + 1;
            minionCountPerPhase = smallMonsterCount / totalPhase;

            StartCoroutine(ProgressStage(currentStageInfo.f_stageDelay, smallMonsterCount));
            return true;
        }

        public Enemy Launch(Defines.EnemyType enemyType)
        {
            if (!CanLaunchAble())
                return null;

            MonsterInfoTable.Row currentMonsterTable = null;
            Cell targetCell = null;

            switch (enemyType)
            {
                case Defines.EnemyType.Minion:
                    currentMonsterTable = RandomEx.Range(smallMonsterList);
                    break;
                case Defines.EnemyType.MiddleBoss:
                    currentMonsterTable = middleBossList[currentPhase];
                    break;
                case Defines.EnemyType.Boss:
                    currentMonsterTable = bossInfo;
                    break;
            }

            targetCell = board.GetAvailableCell(enemyType, currentMonsterTable.LineType);
            if (targetCell == null)
                return null;

            Enemy enemy = enemyPools.Pop().Launch(startingPoint.position, targetCell, player, currentMonsterTable);
            currentIndex++;
            currentEnemies.Add(enemy);

            return enemy;
        }

        public Sprite GetEnemySprite(string name)
        {
            if(!loadedSprite.ContainsKey(name))
            {
                loadedSprite.Add(name, Resources.Load<Sprite>(string.Format("Image/Enemy/{0}", name)));
            }
            return loadedSprite[name];
        }

        public bool CanLaunchAble()
        {
            return currentIndex < enemyPools.Count;
        }

        private Enemy GetEnemy()
        {
            List<Enemy> aliveList = currentEnemies.FindAll(x => x.EnemyMode != Enemy.Mode.Dead);

            if (aliveList.Count == 0)
                return null;
            else
                return RandomEx.Range(aliveList);
        }

        public Enemy GetEnemy(Board.LineType lineType)
        {
            List<Enemy> aliveList = currentEnemies.FindAll(x => x.EnemyMode != Enemy.Mode.Dead && x.Row.LineType == lineType);
            if(aliveList.Count == 0)
                return GetEnemy();
            else
                return RandomEx.Range(aliveList);
        }

        public void CollectEnemy(Enemy enemy)
        {
            currentEnemies.Remove(enemy);
            enemyPools.Push(enemy);
            // 불렛 참조 하여 적 스텍 형으로 다시 만들어야 한다.
            currentIndex--;
        }

        public List<BattleObject> GetAllEnemy()
        {
            List<BattleObject> casted = currentEnemies.Cast<BattleObject>().ToList();
            return casted;
        }

        public List<BattleObject> GetAllEnemyByCells(List<Cell> detectedCellList)
        {
            List<BattleObject> casted = new List<BattleObject>();
            foreach (var enemy in currentEnemies)
            {
                if(detectedCellList.Contains(enemy.RefCell))
                {
                    casted.Add(enemy.BattleObject);
                }
            }

            return casted;
        }
    }
}
