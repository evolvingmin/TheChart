using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 1. 맵 배경으로 깔리는 운석들은 크기에 따라 속도가 가변한다 
	- 큰것들은 느리고, 작은 것 들은 빠르다. (이것들도 가변영역)
	- 처음에 속도가 정해지면, 바뀌지 않는다.
	
2. 회전에 관해서 
	- 50%는 회전하지 않는다
		- 처음에 고유한 방향값을 가진다.
			§ 1~360사이의 값을 가지고 그걸로 정해지면 그 회전값을 고정으로 가지고 내려온다
	- 50%는 회전한다.
		- 속도에 폭이 있고, 기준속도를 기본으로, (큰애는 느리고, 작은애는 빠르게)
			§ 회전 속도도 가변영역이 있고, 그 사이를 픽하면 그걸로 고정으로 해서 회전한다.
	3. 시작점과 끝점
		- 윗 상단 가로를 0~100으로 가정하고, 그 사이를 랜덤으로 하여 시작 값을 정한다.

    	4. 이동방향
		a. 운석이 스폰 된 위치를 기준으로 하여 아래로 내려갈 것이며, 진행방향을 기준으로 좌우 60도, 총합 120도의 각도를 랜덤으로 선택한다.
	5. 사라지는 것
		a. 운석이 카메라의 영역에서 벗어날 경우, 자동적으로 사라진다( 프로그램에서 기능으로 구현함)
		b. 이때 시야에 사라짐을 의미하는 것은, 그 기준을 중심으로 잡는 것이 아니라, 직관적으로 이미지 전체가 사라져야 함을 의미한다.
	6. 생성 주기
		
	7. 위 사항에서 언급한 각종 수치의 범위값은 유니티의 인스펙터에서 설정할 수 있도록 만든다. 
	8. 운석량에 대한 개념
	스테이지의 운석양을 기준으로 
	해당 양보다 적다면 운석 등장빈도 * 1.5
해당 양보다 많다면 운석 등장빈도 * 0.5
    9. 생성주기 : 기본주기는 세팅할 수 있도록 해주기. 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    todo== 운석들이 어느정도 생성된 것을 기본 장면으로 하고 싶다
    // 
    */

namespace StarShip
{
    public class DebrisGenerator : MonoBehaviour
    {
        [System.Serializable]
        public struct AppearData
        {
            public float ratio;
            public float timeMinRange;
            public float timeMaxRange;
        }

        public int MAX_STACK = 50;
        private Stack<Debris> debrisStack = null;

        [SerializeField]
        private BoxCollider2D screenCollider = null;

        [SerializeField]
        private GameObject DebrisPrefab = null;

        //private float speedForScaleFactor = 0.0f;

        [SerializeField]
        private float minScaleFactor = 1.0f;
        [SerializeField]
        private float maxScaleFactor = 5.0f;
        [SerializeField]
        private float baseSpeed = 5.0f;
        [SerializeField]
        private float baseScale = 5.0f;

        [SerializeField]
        private float debrisCollectRange = 2.5f;

        [SerializeField]
        private Sprite[] debrisImage = null;

        [SerializeField]
        private AppearData[] appearDatas = null;

        private bool isOn = true;

        // 속도는 = 1 / 스케일 펙터
        // Use this for initialization

        IEnumerator TestIteration()
        {
            float nextAppearTime = 0.0f;
            while(isOn)
            {
                ReleaseDebris();
                float resultRatio = Random.Range(0, 1.0f);
                float baseRatio = 0.0f;
                foreach (var item in appearDatas)
                {
                    baseRatio = item.ratio;
                    if (resultRatio <=baseRatio)
                    {
                        nextAppearTime = Random.Range(item.timeMinRange, item.timeMaxRange);
                        break;
                    }
                }
                yield return new WaitForSeconds(nextAppearTime);
            }
        }
        private void Awake()
        {
            Initialize();
            StartCoroutine(TestIteration());
        }

        public bool Initialize()
        {
            debrisStack = new Stack<Debris>(MAX_STACK);
            for (int i = 0; i < MAX_STACK; i++)
            {
                Debris debris = Instantiate(DebrisPrefab).GetComponent<Debris>();
                debris.Initialize(OnCollected);
                debris.transform.SetParent(transform);
                debrisStack.Push(debris);
            }
            DebrisPrefab.SetActive(false);
            return true;
        }

        public void ReleaseDebris()
        { 
            Debris debris = OnReleased();
            if(debris == null)
            {
                Debug.Log("No more debris!");
                return;
            }

            debris.Fire(GenerateDebrisData());
        }

        private Debris OnReleased()
        {
            if (debrisStack.Count <= 0)
                return null;

            return debrisStack.Pop();
        }

        private Vector3 GetActualPosition(float startPositionRatio)
        {
            float baseX = screenCollider .transform.position.x - screenCollider.size.x / 2.0f;
            float maxX = screenCollider.transform.position.x + screenCollider.size.x / 2.0f;

            float resultX = Mathf.Lerp(baseX, maxX, startPositionRatio / 100.0f);
            float maxY = screenCollider.transform.position.y + screenCollider.size.y / 2.0f + debrisCollectRange;
            // z 값은 데브리스 제네레이터와 동위로 맞추기 위해서.
            return new Vector3(resultX, maxY, transform.position.z);
        }

        private Debris.Data GenerateDebrisData()
        {
            
            float l_scaleFactor = Random.Range(minScaleFactor, maxScaleFactor);
            float l_speed = baseSpeed / l_scaleFactor;
            float l_rotation = Random.Range(0.0f, 360.0f);
            float l_startPositionRatio = Random.Range(0.0f, 100.0f);
            float l_movingDirectionDegree = Random.Range(-30.0f, 30.0f);
            bool l_isRotationMoving = Random.Range(0, 3) == 0 ? false : true;

            Debris.Data data = new Debris.Data()
            {
                isRotationMoving = l_isRotationMoving,
                speed = l_speed,
                rotation = l_rotation,
                startPositionRatio = l_startPositionRatio,
                actualStartPosition = GetActualPosition(l_startPositionRatio),
                movingDirectionDegree = l_movingDirectionDegree,
                scaleFactor = l_scaleFactor,
                actualScale = l_scaleFactor / baseScale,
                debrisCollectBoundY = screenCollider.transform.position.y - screenCollider.size.y / 2.0f - debrisCollectRange,
                sprite = debrisImage[Random.Range(0, debrisImage.Length)]
            };

            return data;
        }

        public void OnCollected(Debris collected)
        {
            if (debrisStack.Count >= MAX_STACK)
                return;

            debrisStack.Push(collected);
            collected.gameObject.SetActive(false);
        }

    }
}
