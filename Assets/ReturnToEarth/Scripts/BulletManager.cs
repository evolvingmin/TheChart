using System;
using System.Collections;
using System.Collections.Generic;
using ReturnToEarth;
using UnityEngine;

public class BulletManager : MonoBehaviour {

    private ResourceManager resourceManager;

    private const string resourceCategory = "Bullet";

    public Define.Result Initialize(ResourceManager resourceManager)
    {
        this.resourceManager = resourceManager;
        return Define.Result.OK;
    }

    public void Fire(Unit unit, string bullet, Vector3 firePosition, Vector3 normalized)
    {
        GameObject bulletObject = resourceManager.GetObject<GameObject>(resourceCategory, bullet);
        bulletObject.transform.SetParent(transform);

        bulletObject.GetComponent<Bullet>().Fire(firePosition, normalized);
    }
    
    public void Collect(GameObject bulletObject)
    {
        resourceManager.CollectGameObject(resourceCategory, bulletObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Collect(collision.gameObject);
        }
    }

}
