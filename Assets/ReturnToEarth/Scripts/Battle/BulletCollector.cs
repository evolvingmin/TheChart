using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    namespace UnUsed
    {
        public class BulletCollector : MonoBehaviour
        {
            [SerializeField]
            private Collider2D battleBoundCollider;
            
            private void OnTriggerExit2D(Collider2D collision)
            {
                if (collision.gameObject.CompareTag("Bullet"))
                {
                    collision.GetComponent<Bullet>().StartCollect();
                }
            }
            
            
        }

    }
}
