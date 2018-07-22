using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 테이블과 프리펩의 영역
// 테이블은 벨런스에 관련된 모든것을 조절한다
// 프리펩은 외관으로 영향끼치는 모든 것에 관여한다.
// 이제 테이블에 이 이미지를 참고한다 X 이제 없는거
// 테이블에는 프리펩이라는 항목 하나만 남고
// 


namespace StarShip
{
    // 타입가정에서는 타입별로 == 인포와 같게 가도록 하자.
    [Serializable]
    public enum BulletType
    {
        Mass,
        Laser,
        Missile
    }
    // 고친다면 어떤 방향?
    // 불렛마다 모양이 크게 바뀌는건 맞다
    // 프리펩으로 정해진건 더이상 나오지 않도록.
    // 적과 우리편은 사용을 공유하도록 계산하자
    
    [Serializable]
    public struct BulletInfo
    {
        public int maxNumberInPlay;
        public GameObject prefab;
        [HideInInspector]
        public int currentNumberInPlay;
        
        public BulletType type;
    }

    public delegate void BulletTypeDel(BulletType Type);


    [ExecuteInEditMode]
    public class BulletController : MonoBehaviour
    {
        [SerializeField]
        private BulletInfo[] bulletInfos;

        private Dictionary<int, Stack<Bullet>> magazines = new Dictionary<int, Stack<Bullet>>();
        private Dictionary<string, Sprite> loadedSprite = new Dictionary<string, Sprite>();
        private int currentBulletCount = 0;

        private Bullet[] bulletsInPlay;
        private int TotalBulletCount;

        public int CurrentBulletCount {  get { return currentBulletCount; } }

        private void OnValidate()
        {
#if UNITY_EDITOR
            TotalBulletCount = 0;

            for (int i = 0; i < bulletInfos.Length; i++)
            {
                bulletInfos[i].type = (BulletType)i;
            }
#endif
        }

        public bool Initialize()
        {
            TotalBulletCount = 0;
            foreach (var item in bulletInfos)
            {
                TotalBulletCount += item.maxNumberInPlay;
                MagazineInitialize(item);
            }

            bulletsInPlay = new Bullet[TotalBulletCount];

            Debug.Log("Bullet Pre-loaded!");
            
            return true;
        }

        private void MagazineInitialize(BulletInfo item)
        {
            magazines[(int)item.type] = new Stack<Bullet>(item.maxNumberInPlay);
            for (int i = 0; i < item.maxNumberInPlay; i++)
            {
                Bullet bullet = Instantiate(item.prefab).GetComponent<Bullet>();
                bullet.Initialize(magazines[(int)item.type], Collect);
            }
        }

        public void Fire(BattleModifier modifier, string shooterTag, Vector3 startPosition, Vector3 endPosition, Vector3 velocity, string spriteName ="") 
        {

            if (bulletInfos[(int)modifier.StatuesInfo.bulletType].currentNumberInPlay >= bulletInfos[(int)modifier.StatuesInfo.bulletType].maxNumberInPlay)
                return;

            Sprite replaceSprite = null;
            if(spriteName != "")
            {
                if(!loadedSprite.ContainsKey(spriteName))
                    loadedSprite.Add(spriteName, Resources.Load<Sprite>(string.Format("Image/Bullet/{0}", spriteName)));

                if(replaceSprite != loadedSprite[spriteName])
                    replaceSprite = loadedSprite[spriteName];
            }

            bulletsInPlay[currentBulletCount] = magazines[(int)modifier.StatuesInfo.bulletType].Pop();
            bulletsInPlay[currentBulletCount].Fire(modifier, shooterTag, startPosition, endPosition, velocity, replaceSprite);
            currentBulletCount++;
            bulletInfos[(int)modifier.StatuesInfo.bulletType].currentNumberInPlay++;
        }

        void Collect(BulletType type)
        {
            if (currentBulletCount <= 0)
                return;

            bulletInfos[(int)type].currentNumberInPlay--;
            currentBulletCount--;
        }
    }
}



