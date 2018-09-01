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

        private Transform SpriteTransform;
        private SpriteRenderer spriteRenderer;

        private Block currentBlock;

        private UnitState state = UnitState.None;

        private void Awake()
        {
            SpriteTransform = gameObject.transform.GetChild(0);
            spriteRenderer = SpriteTransform.GetComponent<SpriteRenderer>();
        }

        public void Initialize(Vector3 unitScale, Block block, GameManager.Team team)
        {
            currentBlock = block;
            transform.position = currentBlock.transform.position;
            this.team = team;

            if(this.team == GameManager.Team.Opponent)
            {
                transform.LookAt(transform.position + Vector3.down, Vector3.back);
                spriteRenderer.color = Color.blue;
            }
            else
            {
                transform.LookAt(transform.position + Vector3.up, Vector3.back);
            }

            SpriteTransform.transform.localScale = unitScale;

            state = UnitState.Idle;
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
            }
        }

        private void Update()
        {
            switch (state)
            {
                case UnitState.Selected:
                    Vector3 MousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    MousePoint.z = transform.position.z;
                    transform.LookAt(MousePoint, Vector3.back);
                    break;
            }
        }
    }
}

