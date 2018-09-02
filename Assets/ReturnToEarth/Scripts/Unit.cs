using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReturnToEarth
{
    public class Unit : MonoBehaviour
    {
        public enum UnitState
        {
            Idle,
            Selected,
            Moving,
            Attack,
            Dead,
            None
        }

        private GameManager.Team team;
        private BulletManager bulletManager;

        private Transform SpriteTransform;
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        private Block currentBlock;

        private Vector3 cachedTarget;
        private UnitState state = UnitState.None;

        // 이 정보 역시 ScriptTableObject에서 와야 한다.
        private string bullet = "Default";
        private string prefix = "Unit_";

        private Dictionary<UnitState, string> animStrings;

        private void Awake()
        {
            SpriteTransform = gameObject.transform.GetChild(0);
            spriteRenderer = SpriteTransform.GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            bulletManager = GameManager.Instance.BulletManager;
        }

        private void GenerateAnimatorNameState()
        {
            animStrings = new Dictionary<UnitState, string>();

            string[] names = Enum.GetNames(typeof(UnitState));

            for (int i = 0; i < names.Length; i++)
            {
                animStrings.Add((UnitState)i, prefix + name + "_" +names[i]);
            }
        }

        public void Initialize(Vector3 unitScale, Block block, GameManager.Team team)
        {
            currentBlock = block;
            transform.position = currentBlock.transform.position;
            this.team = team;

            if(this.team == GameManager.Team.Opponent)
            {
                LookAtDown();
                spriteRenderer.color = Color.green;
            }
            else
            {
                LookAtUp();
            }

            SpriteTransform.transform.localScale = unitScale;

            state = UnitState.Idle;
            GenerateAnimatorNameState();
            currentBlock.PlaceUnit(this);
        }

        public void UpdateState(Block.BlockState state)
        {
            if (team == GameManager.Team.Opponent)
                return;

            if(state == Block.BlockState.Selected)
            {
                this.state = UnitState.Selected;
            }
            else if ( state == Block.BlockState.Default )
            {
                this.state = UnitState.Idle;
                if (team == GameManager.Team.Player)
                {
                    LookAtUp();
                }
                else
                {
                    LookAtDown();
                }
            }
        }

        private void Update()
        {
            switch (state)
            {
                case UnitState.Selected:
                    LookAtMouse();
                    break;
            }
        }

        private void LookAtUp()
        {
            transform.LookAt(transform.position + Vector3.up, Vector3.back);
        }

        private void LookAtDown()
        {
            transform.LookAt(transform.position + Vector3.down, Vector3.back);
        }

        private void LookAtMouse()
        {
            Vector3 MousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MousePoint.z = transform.position.z;
            transform.LookAt(MousePoint, Vector3.back);

            Fire(MousePoint);
        }

        // 현재 테스트 코드. 정상 플로우가 아니라 총알을 쏘고 회수하는 로직 확인을 위해 우선 개발 해 보도록 한다.
        // 나의 유닛이 선택된 상태에서, 마우스 방향으로 그냥 무조건 총알을 쓰는 로직을 한번 만들어 보도록 한다.

        private void Fire(Vector3 Target)
        {
            cachedTarget = Target;
            animator.SetTrigger("Attack");
            Vector3 Forward = cachedTarget - transform.position;
            bulletManager.Fire(this, bullet, transform.position, Forward.normalized);

        }

        private void EventFire()
        {
            //Vector3 Forward = cachedTarget - transform.position;
            //bulletManager.Fire(this, bullet, transform.position, Forward.normalized);
        }
    }
}

