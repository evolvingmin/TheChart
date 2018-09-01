using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StarShip
{
    public class Weapon : MonoBehaviour
    {
        public enum Mode
        {
            OFF,
            IDLE,
            ROTATE,
            FIRE
        }

        private Mode mode = Mode.OFF;

        [SerializeField]
        private bool usePrefab = false;

        [SerializeField]
        private Transform firePoint;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private Enemy currentTarget;

        private TableHandler.Row table;
        private DataProvider dataProvider;

        private float elapsedTime = 0.0f;

        private const float SEARCH_SPEED = 0.2f;

        private Board.LineType lineType;

        public string bulletAppearance;
        private Animator weaponAnimator;

        private BattleModifier modifier;

        private void Awake()
        {
            weaponAnimator = GetComponent<Animator>();
            
        }

        private void Update()
        {

            elapsedTime += Time.deltaTime;
            switch (mode)
            {
                case Mode.IDLE:
                    if (elapsedTime > SEARCH_SPEED)
                    {
                        SetLock();
                        elapsedTime = 0.0f;
                    }
                    break;
                case Mode.FIRE:
                    if(elapsedTime > modifier.StatuesInfo.attackSpeed)
                    {
                        Fire();

                        elapsedTime = 0.0f;
                    }
                    break;
            }
        }

        public bool Initialize(TableHandler.Row table, Mode _mode= Mode.IDLE)
        {
            gameObject.SetActive(true);
            mode = _mode;
            this.table = table;
            dataProvider = new DataProvider("WeaponList", table);
            if (table == null)
            {
                spriteRenderer.enabled = false;
                return false;
            }
            spriteRenderer.enabled = true;

            if (usePrefab)
            {

            }
            else
            {
                bulletAppearance = dataProvider.Get<string>("bullettype");
                spriteRenderer.sprite = Resources.Load<Sprite>(string.Format("Image/Slot/Weapon/{0}/{1}",
                    dataProvider.Get<string>("weapontype"),
                    dataProvider.Get<string>("icon")));
            }
            modifier = new BattleModifier();
            modifier.GenerateDataFromWeapon(dataProvider);
            
            lineType = (Board.LineType)Enum.Parse(typeof(Board.LineType), dataProvider.Get<string>("attacktarget"), true);
            return true;
        }

        public void SetLock()
        {
            Enemy target = GameManager.Instance.EnemyGenerator.GetEnemy(lineType);
            if(target != null && table != null)
            {
                currentTarget = target;
                mode = Mode.FIRE;
                elapsedTime = 0.0f;
            }
        }

        public void NotifyCurrentEnemyDead()
        {
            currentTarget = null;
            mode = Mode.IDLE;
        }

        public void Fire()
        {
            Quaternion to = Quaternion.LookRotation(currentTarget.transform.position - transform.position, Vector3.back);
            to.x = 0.0f;
            to.y = 0.0f;
            transform.rotation = to;
            Vector3 bulletVelocity = ( currentTarget.transform.position - transform.position ).normalized * modifier.StatuesInfo.bulletSpeed;
            GameManager.Instance.BulletController.Fire(modifier, "Player", firePoint.position, currentTarget.transform.position, bulletVelocity, (usePrefab == false) ? bulletAppearance : "" );

            if(weaponAnimator != null)
                weaponAnimator.SetTrigger("OnFire");
        }

        public static Weapon GetWeaponByPrefab(TableHandler.Row table)
        {
            string path = "Prefabs/Weapon/{0}";

            //일단 타겟 프리펩 대신에 요거 쓰도록 한다.
            string prefabName = table.Get<string>("weapon_appearance");
            var prefab = Resources.Load(string.Format(path, prefabName));

            return ( (GameObject)Instantiate(prefab) ).GetComponent<Weapon>();
        }
    }

}
