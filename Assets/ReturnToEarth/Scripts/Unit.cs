using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReturnToEarth
{
    public class Unit : MonoBehaviour
    {

        public enum Team
        {
            None,
            Friendly,
            Enemy
        }

        private Team team;

        private Transform SpriteTransform;
        private SpriteRenderer spriteRenderer;

        //private Block currentBlock;

        private void Awake()
        {
            SpriteTransform = gameObject.transform.GetChild(0);
            spriteRenderer = SpriteTransform.GetComponent<SpriteRenderer>();
        }

        public void Initialize(Vector3 unitScale, Sprite sprite,  Block block, Team _team)
        {
            spriteRenderer.sprite = sprite;
            //currentBlock = block;

            team = _team;

            if(team == Team.Enemy)
            {
                transform.Rotate(0, 0, 180.0f);
            }
            SpriteTransform.transform.localScale = unitScale;

            //Debug.Log("Unit Was Initialized, " + currentBlock);
        }
    }
}

