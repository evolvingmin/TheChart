using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    public class Enemy : MonoBehaviour
    {
        public enum Mode
        {
            Moving,
            Attack,
            Dead
        }

        public float speed = 5.0f;

        public Mode EnemyMode {  get { return mode; } }
        private Mode mode = Mode.Dead;
        private Vector3 directionVec;
        private Vector3 destPosition;
        private StarShip starShip = null;

        public MonsterInfoTable.Row Row
        { get { return row; } }
        private MonsterInfoTable.Row row;
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        public Cell RefCell {  get { return cell; } }
        private Cell cell = null;

        public BattleObject BattleObject {  get { return battleObject; } }
        private BattleObject battleObject;

        private BattleModifier battleModifier;

        private const float DISTANCE_CLOSE_ENOUGH = 0.15f;
        public void Update()
        {
            switch (mode)
            {
                case Mode.Moving:
                    directionVec = ( destPosition - transform.position ).normalized;
                    transform.position += directionVec * row.f_moveSpeed * Time.deltaTime;
                    if(Vector3.Distance(transform.position, destPosition) <= DISTANCE_CLOSE_ENOUGH)
                    {
                        mode = Mode.Attack;
                        StartCoroutine(Fire());
                    }
                    break;
                case Mode.Attack:
                    break;
            }
        }

        public Enemy Launch(Vector3 startPosition, Cell cell, StarShip starShip = null, MonsterInfoTable.Row table = null)
        {
            transform.position = startPosition;
            directionVec = ( cell.Center - startPosition ).normalized;
            destPosition = cell.Center;
            this.cell = cell;
            cell.Occupy(this);

            if (mode == Mode.Dead)
            {
                Initialize(startPosition, cell, starShip, table);
            }
            mode = Mode.Moving;
            return this;
        }

        public bool Initialize(Vector3 startPosition, Cell cell, StarShip starShip, MonsterInfoTable.Row table)
        {
            this.starShip = starShip;
            this.row = table;


            spriteRenderer.sprite = GameManager.Instance.EnemyGenerator.GetEnemySprite(table.s_appearance);
            //table.Width

            int[] Size = table.Size;
            transform.localScale = new Vector3(Size[0], Size[1], 1);

            gameObject.SetActive(true);

            battleObject = new BattleObject_Enemy();
            battleObject.Initialize(table,this);
            battleModifier = new BattleModifier();
            battleModifier.GenerateDataFromEnemy(table);

            return true;
        }

        IEnumerator Fire()
        {
            while (mode == Mode.Attack)
            {
                Vector3 bulletVelocity = ( starShip.transform.position - transform.position ).normalized * row.f_bulletSpeed;
                GameManager.Instance.BulletController.Fire(battleModifier, "Enemy", transform.position, starShip.transform.position, bulletVelocity);
                yield return new WaitForSeconds(row.f_attackSpeed);
            }

        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Bullet"))
            {
                return;
            }

            Bullet bullet = collision.GetComponent<Bullet>();

            if (bullet.ShooterTag != "Player")
                return;

            if (bullet.Hit(battleObject))
            {
                int actualDamage = battleObject.GetActualDamage(bullet.Modifier);

                battleObject.TakeTrueDamage(actualDamage);
            }
        }


        public void Collect()
        {
            mode = Mode.Dead;
            cell.Release();
            gameObject.SetActive(false);
            GameManager.Instance.EnemyGenerator.CollectEnemy(this);
            GameManager.Instance.StarShip.NotifyCurrentEnemyDead();

        }

    }
}
