using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MovementEffects;

[Serializable]
public class PooledPrefab
{
    public string Name;
    public PooledPrefabs Type; 
    public int PreBuild;
    public GameObject Prefab;
    public List<GameObject> AllObjects = new List<GameObject>();
    public List<GameObject> AvailableObjects = new List<GameObject>();
}

[Serializable]
public class PrefabPoolManager
{
    [SerializeField] PooledPrefab[] pooledPrefabs;
    [Space(20)]
    [SerializeField] Transform prefabRepository;

    string poolTag = "PoolTag";
    Dictionary<PooledPrefabs, PooledPrefab> prefabs = new Dictionary<global::PooledPrefabs, PooledPrefab>();

    public void Start()
    {
        Timing.Instance.AddTag(poolTag, true);
        prefabs = pooledPrefabs.GroupBy(x => x.Type).ToDictionary((x => x.Key), (x => x.First()));
        AppManager.Instance.OnLoadLevel += ForceDisable;

        PreBuild();
    }

    void PreBuild()
    {
        foreach (var p in pooledPrefabs)
        {
            for (int i = 0; i < p.PreBuild; i++)
            {
                var g = AddNew(p.Type);
                SetParent(g, p);
            }
        }
    }

    public GameObject GetPooledPrefab(PooledPrefabs prefab)
    {
        if (!prefabs.ContainsKey(prefab))
            Debug.LogErrorFormat("No PooledPrefab of type {0} is registered!", prefab);

        var pPref = prefabs[prefab];

        GameObject g;
        if (pPref.AvailableObjects.Count == 0)
        {
            g = AddNew(prefab);
        }
        else
        {
            g = pPref.AvailableObjects.First();
        }
        pPref.AvailableObjects.Remove(g);
        g.SetActive(true);
        g.transform.parent = null;

        return g;
    }
    public GameObject GetPooledPrefabTimed(PooledPrefabs prefab, float t)
    {
        var g = GetPooledPrefab(prefab);
        Timing.Instance.CallDelayedOnInstance(t, () => { DeactivatePrefab(g); });
        return g;
    }
    public bool DeactivatePrefab(GameObject prefab)
    {
        var pPrefabs = prefabs.Values.Where(x => x.AllObjects.Contains(prefab)).ToArray();
        if (pPrefabs.Length != 0)
        {
            var pPrefab = pPrefabs[0];
        
            SetParent(prefab, pPrefab);
            if(!pPrefab.AvailableObjects.Contains(prefab))
                pPrefab.AvailableObjects.Add(prefab);

            return true;
        }
        else
            return false;
    }

    void SetParent(GameObject go, PooledPrefab prefab, bool deactivate = true)
    {
        var parent = GetOrCreateGO(prefabRepository, prefab.Type.ToString());
        go.transform.parent = parent.transform;

        if (deactivate)
            go.SetActive(false);
    }
    GameObject AddNew(PooledPrefabs type)
    {
        var p = prefabs[type];
        var g = UnityEngine.Object.Instantiate(p.Prefab);
        p.AllObjects.Add(g);
        p.AvailableObjects.Add(g);
        return g;
    }
    GameObject GetOrCreateGO(Transform root, string name)
    {
        var f = root.Find(name);
        if (f != null)
            return f.gameObject;
        else
        {
            var g = new GameObject(name);
            g.transform.parent = root;
            return g;
        }
    }

    public void ForceDisable()
    {
        foreach(var prefs in prefabs.Values)
        {
            foreach(var o in prefs.AllObjects.Except(prefs.AvailableObjects))
            {
                DeactivatePrefab(o);
            }
        }
    }
}
