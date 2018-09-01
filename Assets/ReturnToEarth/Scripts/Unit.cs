using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReturnToEarth
{
    public class Unit : MonoBehaviour
    {
        private GameManager.Team team;

        private Transform SpriteTransform;
        private SpriteRenderer spriteRenderer;

        private Block currentBlock;

        private void Awake()
        {
            SpriteTransform = gameObject.transform.GetChild(0);
            spriteRenderer = SpriteTransform.GetComponent<SpriteRenderer>();
        }

        //임시로 작성됨.
        private void SetSpriteRandomly()
        {

        }

        public void Initialize(Vector3 unitScale, Block block, GameManager.Team team)
        {
            currentBlock = block;
            transform.position = currentBlock.transform.position;
            this.team = team;

            if(this.team == GameManager.Team.Opponent)
            {
                transform.Rotate(0, 0, 180.0f);
            }

            SpriteTransform.transform.localScale = unitScale;

            SetSpriteRandomly();
        }


    }
}

