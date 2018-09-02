using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UnityObjectPool
{
    private string name;
    private UnityEngine.Object basePrefab;
    private Stack<UnityEngine.Object> available;

    // todo : 리소스 메니져는 나중에 ini나 다른 파일과 연동하여 해당 ObjectPool 의 baseSize를 조절 할 수 있도록 하겠다.
    private int baseSize = 10;
    private int currentSize = 0;

    private ResourceManager resourceManager;

    public UnityObjectPool(string name, UnityEngine.Object basePrefab, ResourceManager resourceManager)
    {
        this.basePrefab = basePrefab;
        this.name = name;
        this.resourceManager = resourceManager;

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
            spawned.name = name;
            if (spawned is GameObject)
            {
                GameObject current = (GameObject)spawned;
                current.SetActive(false);
                current.transform.SetParent(resourceManager.transform);
            }
            available.Push(spawned);
        }
        currentSize = baseSize;
    }

    public UnityEngine.Object GetAvailableObject()
    {
        if(basePrefab == null)
        {
            Debug.LogWarning("basePrefab was not Loaded, name : " + name);
            return null;
        }

        UnityEngine.Object result = null;

        if(available.Count > 0)
        {
            result = available.Pop();
        }
        else
        {
            int baseSize = Mathf.Max(1, currentSize);
            IncreaseSizeBy(baseSize / 2);
            result = available.Pop();
        }

        if (result is GameObject)
        {
            ( (GameObject)result ).SetActive(true);
        }

        return result;
    }

    private void IncreaseSizeBy(int amount)
    {
        for(int i =0; i< amount; i++)
        {
            UnityEngine.Object spawned = resourceManager.Spawn(basePrefab);
            spawned.name = name;
            if (spawned is GameObject)
            {
                GameObject current = (GameObject)spawned;
                current.SetActive(false);
                current.transform.SetParent(resourceManager.transform);
            }

            available.Push(spawned);
        }
        currentSize += amount;
    }

    public Define.Result CollectGameObject(GameObject collected)
    {
        collected.SetActive(false);
        collected.transform.SetParent(resourceManager.transform);
        available.Push(collected);

        return Define.Result.OK;
    }

    public override string ToString()
    {
        int used = currentSize - available.Count;
        return string.Format("[{0} : ({1}/{2})]", name, used, currentSize);
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

    public Define.Result CollectGameObject(GameObject collected)
    {
        if (!pools.ContainsKey(collected.name))
        {
            return Define.Result.NOT_INITIALIZED;
        }

        return pools[collected.name].CollectGameObject(collected);
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append(string.Format("<{0}>", category));
        stringBuilder.AppendLine();
        foreach (var item in pools)
        {
            stringBuilder.Append(item.Value);
            stringBuilder.AppendLine();
        }
        return stringBuilder.ToString();
    }
}

public class ResourceManager : MonoBehaviour {

    //public delegate void SpawnAction<T>(T spawned);

    private Dictionary<string, UnityObjectCollection> collections;

    public string RootPath {  get { return rootPath; } }
    private string rootPath = "Prefabs";

    public Define.Result Initialize(string rootPath = "Prefabs")
    {
        this.rootPath = rootPath;
        collections = new Dictionary<string, UnityObjectCollection>();

        return Define.Result.OK;
    }

    public T SpawnObject<T>(string category, string PrefabName) where T : UnityEngine.Object
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

    public Define.Result CollectGameObject(string resourceCategory, GameObject collected)
    {
        if(!collections.ContainsKey(resourceCategory))
        {
            return Define.Result.NOT_INITIALIZED;
        }

        return collections[resourceCategory].CollectGameObject(collected);
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();

        foreach (var collection in collections)
        {
            stringBuilder.Append(collection.Value);
            stringBuilder.AppendLine();
        }
        return stringBuilder.ToString();
    }

    public void ShowCurrentObjects()
    {
        Debug.Log(ToString());
    }
}
