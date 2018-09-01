using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace StarShip
{
    public class Debris : MonoBehaviour
    {

        public delegate void DebrisCallback(Debris self);



        [Serializable]
        public struct Data
        {
            public bool isRotationMoving;
            public float speed;
            [Range(1, 360)]
            public float rotation;
            [Range(0, 1)]
            public float startPositionRatio;
            public Vector3 actualStartPosition;
            [Range(-60, 60)]
            public float movingDirectionDegree;
            public Vector3 directionVector;
            [Range(1, 5)]
            public float scaleFactor;
            public float actualScale;
            public Sprite sprite;

            public float debrisCollectBoundY;
        }

        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        private Data data;
        private DebrisCallback onDeactivated;



        // 잔해는 아래쪽으로 내려가는 걸 기본 가정으로 함. 그럼 아래쪽으로 내려가는 걸 작성 해 보자.
        private void Update()
        {
            if (data.debrisCollectBoundY > transform.position.y)
            {
                if (onDeactivated != null)
                    onDeactivated(this);
            }

            if (data.isRotationMoving)
            {
                // 로테이션 하는 속도가 너무 느리다.
                // 
                transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + data.rotation / ( data.scaleFactor + 1 ) * Time.deltaTime);
            }


            //디렉션 백터로 내려가자
            transform.position -= data.directionVector * data.speed * Time.deltaTime;

        }


        public bool Initialize(DebrisCallback _onDeactivated)
        {
            onDeactivated = _onDeactivated;
            gameObject.SetActive(false);
            return true;
        }

        public void Fire(Data _data)
        {
            data = _data;
            transform.position = data.actualStartPosition;
            transform.rotation = Quaternion.Euler(0, 0, data.rotation);
            transform.localScale = new Vector3(data.actualScale, data.actualScale);
            spriteRenderer.sprite = data.sprite;

            //x = cos(yaw) * cos(pitch)
            //y = sin(yaw) * cos(pitch)
            data.directionVector = new Vector3((float)( Mathf.Cos(Mathf.Deg2Rad * ( data.movingDirectionDegree + 90.0f )) * Math.Cos(0) ),
                                             (float)( Mathf.Sin(Mathf.Deg2Rad * ( data.movingDirectionDegree + 90.0f )) * Math.Cos(0) ), 0.0f);
            //Debug.Log("used movingDirectionDegree : " + data.movingDirectionDegree);
            //Debug.Log("Generated DireactionVector : " + data.directionVector);
            gameObject.SetActive(true);
        }
    }
}