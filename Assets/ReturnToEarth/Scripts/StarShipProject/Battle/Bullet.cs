using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    public class Bullet : MonoBehaviour
    {
        public enum Mode
        {
            Fire,
            Moving,
            Hit,
            Missed
        }

        private Stack<Bullet> magazine;
        public string ShooterTag { get { return shooterTag; } }
        private string shooterTag = null;
        private Vector3 velocity;
        private Vector3 startPosition;
        private Vector3 endPosition;
        private bool isHit = false;

        private Mode mode;
        private BulletTypeDel bullectCollect;
        private BulletType currentBulletType;

        private SpriteRenderer spriteRenderer = null;
        private Animator animator;

        public BattleModifier Modifier { get { return modifier; } }
        private BattleModifier modifier;

        [SerializeField]
        private AnimationClip OnImpact;

        [SerializeField]
        private Sprite baseImage;

        [SerializeField]
        private LineRenderer LaserLine = null;

        [SerializeField]
        private float lifeTime = 5.0f;

        [SerializeField]
        private bool isDirectional = false;

        public bool Initialize(Stack<Bullet> magazine, BulletTypeDel bullectCollect)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            this.magazine = magazine;
            this.bullectCollect = bullectCollect;
            gameObject.SetActive(false);
            magazine.Push(this);
            velocity = Vector3.zero;

            if (LaserLine != null)
            {
                LaserLine.sortingLayerName = "Bullet";
            }

            return true;
        }

        private void Update()
        {
            switch (currentBulletType)
            {
                case BulletType.Mass:
                case BulletType.Missile:
                    transform.position += velocity * Time.deltaTime;
                    break;
                case BulletType.Laser:
                    if (isHit)
                        return;
                    transform.position += velocity * Time.deltaTime;
                    LaserLine.SetPosition(1, transform.position);
                    break;
            }
        }

        public void Fire(BattleModifier modifier, string shooterTag, Vector3 startPosition, Vector3 endPosition, Vector3 velocity, Sprite replacedSprite = null)
        {
            isHit = false;
            this.modifier = modifier;
            currentBulletType = modifier.StatuesInfo.bulletType;

            switch (currentBulletType)
            {
                case BulletType.Mass:
                case BulletType.Missile:
                    // LaserLine.enabled = false;
                    spriteRenderer.sprite = replacedSprite ?? baseImage;
                    break;
                case BulletType.Laser:
                    this.endPosition = endPosition;
                    this.startPosition = startPosition;
                    LaserLine.SetPosition(0, startPosition );
                    LaserLine.SetPosition(1, startPosition );
                    break;
            }

            gameObject.transform.position = startPosition;
            gameObject.SetActive(true);
            this.shooterTag = shooterTag;
            this.velocity = velocity;

            if (isDirectional)
            {
                Quaternion to = Quaternion.LookRotation(velocity, Vector3.forward);
                to.x = 0;
                to.y = 0;
                transform.rotation = to;
            }
            mode = Mode.Fire;
            animator.SetTrigger("OnFire");
        }
        Vector3 endTailPosition;
        public bool Hit(BattleObject battleObject)
        {
            if (mode == Mode.Hit || mode == Mode.Missed)
                return false;

            if (!battleObject.IsHit(modifier))
            {
                mode = Mode.Missed;
                return false;
            }

            spriteRenderer.enabled = true;
            mode = Mode.Hit;
            animator.SetTrigger("OnHit");
            if (currentBulletType == BulletType.Mass )
                this.velocity /= 5.0f;
            StartCollect();
            return true;
        }

        public void StartCollect()
        {
            if (isHit)
                return;

            isHit = true;
            endPosition = transform.position;
            endTailPosition = startPosition;
            StartCoroutine(StartFadeIn());
        }

        private void ProcessCollect()
        {
           
            bullectCollect.Invoke(currentBulletType);
            gameObject.SetActive(false);
            shooterTag = "";
            velocity = Vector3.zero;
            magazine.Push(this);
        }

        IEnumerator StartFadeIn()
        {
            switch (currentBulletType)
            {
                case BulletType.Mass:
                case BulletType.Missile:
                    yield return new WaitForSeconds(1.0f);
                    ProcessCollect();
                    break;
                case BulletType.Laser:
                    while(isHit)
                    {
                        endTailPosition += velocity * Time.fixedDeltaTime;
                        LaserLine.SetPosition(0, endTailPosition );

                        LaserLine.SetPosition(1, endPosition);
                        //Debug.Log("StartPosition 0 " + endTailPosition);
                        //Debug.Log("StartPosition 1 " + endPosition);
                        if (Vector3.Distance(endPosition, endTailPosition) <= 1f)
                        {
                            ProcessCollect();
                            yield break;
                        }
                        else
                        {
                            yield return new WaitForFixedUpdate();
                        }
                    }

                    break;
                
            }
        }
    }
}
