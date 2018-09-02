using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ReturnToEarth
{
    public class Bullet : MonoBehaviour
    {
        public enum BulletState
        {
            Moving,
            Hit,
            None
        }

        private float speed = 8.0f; // 이런 정보는 ScriptableObject에서 와야 한다.
        private Vector3 forward;

        private BulletState state = BulletState.None;

        public void Fire(Vector3 Origin, Vector3 forward)
        {
            transform.position = Origin;
            this.forward = forward;
            transform.LookAt(Origin + this.forward, Vector3.back);
            state = BulletState.Moving;
        }

        private void Update()
        {
            if (state != BulletState.Moving)
                return;

            transform.position += speed * forward * Time.deltaTime;
        }
    }
}