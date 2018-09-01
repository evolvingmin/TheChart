using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityObjectPool
{
    private string name;
    private UnityEngine.Object basePrefab;
    private Stack<UnityEngine.Object> loaded;
    private Stack<UnityEngine.Object> available;

    // 리소스 메니져는 나중에 ini나 다른 파일과 연동하여
    // 해당 ObjectPool 의 baseSize를 조절 할 수 있도록 하겠다.
    private int baseSize = 10; 

    private ResourceManager resourceManager;

    public UnityObjectPool(string name, UnityEngine.Object basePrefab, ResourceManager resourceManager)
    {
        this.basePrefab = basePrefab;
        this.name = name;
        this.resourceManager = resourceManager;

        loaded = new Stack<UnityEngine.Object>();
        available = new Stack<UnityEngine.Object>();

        SpawnUptoBaseSize();
    }

    private void SpawnUptoBaseSize()
    {
        if (basePrefab == null)
        {
            Debug.LogWarning("basePrefab was not Loaded, name : " + name);
            return;
        }

        for (int i = 0; i < baseSize; i++)
        {
            UnityEngine.Object spawned = resourceManager.Spawn(basePrefab);
            available.Push(spawned);
        }
    }

    public UnityEngine.Object GetAvailableObject()
    {
        if(basePrefab == null)
        {
            Debug.LogWarning("basePrefab was not Loaded, name : " + name);
            return null;
        }

        UnityEngine.Object result = available.Pop();

        if(result == null)
        {
            int currentSize = Mathf.Max(1, loaded.Count);
            IncreaseSizeBy(currentSize / 2);
            result = available.Pop();
        }

        
        loaded.Push(result);

        return result;
    }

    private void IncreaseSizeBy(int amount)
    {
        for(int i =0; i< amount; i++)
        {
            UnityEngine.Object spawned = resourceManager.Spawn(basePrefab);
            available.Push(spawned);
        }
    }
}

public class UnityObjectCollection
{
    private string category;
    private Dictionary<string, UnityObjectPool> pools;
    private ResourceManager resourceManager;

    public UnityObjectCollection(string category, ResourceManager resourceManager)
    {
        pools = new Dictionary<string, UnityObjectPool>();
        this.resourceManager = resourceManager;
        this.category = category;
    }

    public T GetObject<T>(string prefabName) where T : UnityEngine.Object
    {
        if(!pools.ContainsKey(prefabName))
        {
            string TargetPath = string.Format("{0}/{1}/{2}", resourceManager.RootPath, category, prefabName);
            T basePrefab = (T)Resources.Load(TargetPath);
            pools.Add(prefabName, new UnityObjectPool(prefabName, basePrefab, resourceManager));
        }
        UnityObjectPool targetObjectPool = pools[prefabName];

        return (T)targetObjectPool.GetAvailableObject();
    }
}

public class ResourceManager : MonoBehaviour {

    private Dictionary<string, UnityObjectCollection> collections;

    public string RootPath {  get { return rootPath; } }
    private string rootPath = "Prefabs";

    public bool Initialize(string rootPath = "Prefabs")
    {
        this.rootPath = rootPath;
        collections = new Dictionary<string, UnityObjectCollection>();

        return true;
    }

    public T GetObject<T>(string category, string PrefabName) where T : UnityEngine.Object
    {
        if(!collections.ContainsKey(category))
        {
            collections.Add(category, new UnityObjectCollection(category, this));
        }

        UnityObjectCollection TargetCollection = collections[category];

        return TargetCollection.GetObject<T>(PrefabName);
    }

    public UnityEngine.Object Spawn(UnityEngine.Object prefab)
    {
        return Instantiate<UnityEngine.Object>(prefab);
    }
}
