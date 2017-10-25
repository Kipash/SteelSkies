using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SteelSkies
{
    [Serializable]
    public class PooledPrefabInfo
    {
        public string Name;
        public PooledPrefabs Type;
        public int PreBuild;
        public GameObject Prefab;
        public PooledPrefab[] AllObjects;
    }

    [Serializable]
    public class PooledPrefab
    {
        public GameObject Instance;
        public bool InUse;
    }

    [Serializable]
    public class PrefabPoolManager
    {
        [SerializeField] PooledPrefabInfo[] pooledPrefabs;
        [Space(20)]
        [SerializeField]
        Transform prefabRepository;

        Dictionary<PooledPrefabs, PooledPrefabInfo> prefabs = new Dictionary<global::PooledPrefabs, PooledPrefabInfo>();
        Dictionary<GameObject, PooledPrefab> instances = new Dictionary<GameObject, PooledPrefab>();

        PooledPrefabInfo[] pPrefabs;
        PooledPrefabInfo pPrefabInfo;
        //GameObject g;
        Transform t;
        int x;
        int y;
        int z;

        public void Initialize()
        {
            prefabs = pooledPrefabs.GroupBy(x => x.Type).ToDictionary((x => x.Key), (x => x.First()));
            AppServices.Instance.SceneManager.OnSceneChanged += SceneManager_OnSceneChanged;

            PreBuild();
        }

        private void SceneManager_OnSceneChanged(Scenes newScene, Scenes oldScene)
        {
            ForceDisable();
        }

        void PreBuild()
        {
            for (x = 0; x < pooledPrefabs.Length; x++)
            {
                //if (pooledPrefabs[x].PreBuild == 0)
                //    continue;

                pooledPrefabs[x].AllObjects = new PooledPrefab[pooledPrefabs[x].PreBuild];
                tempGOs = new List<PooledPrefab>();
                for (y = 0; y < pooledPrefabs[x].PreBuild; y++)
                {
                    GameObject g = AddNew(pooledPrefabs[x]);
                    SetParent(g, pooledPrefabs[x]);
                }
                pooledPrefabs[x].AllObjects = tempGOs.ToArray();
            }
        }

        public GameObject GetPooledPrefab(PooledPrefabs prefab)
        {
            GameObject g;

            if (!prefabs.ContainsKey(prefab))
                UnityEngine.Debug.LogErrorFormat("No PooledPrefab of type {0} is registered!", prefab);

            pPrefabInfo = prefabs[prefab];

            if (pPrefabInfo.AllObjects.Length == 0)
            {
                Debug.LogErrorFormat("No available PooledPrefab of type {0}!", prefab);
                return null;
            }
            else
            {
                g = GetAvailableGO(pPrefabInfo);
                if (g == null)
                {
                    Debug.LogErrorFormat("No available PooledPrefab of type {0}!", prefab);
                    return null;
                }
            }

            g.SetActive(true);
            g.transform.SetParent(null);
            return g;
        }
        public GameObject GetPooledPrefabTimed(PooledPrefabs prefab, float t)
        {
            GameObject g = GetPooledPrefab(prefab);
            AppServices.Instance.StartCoroutine(CommonCoroutine.CallDelay(() => { DeactivatePrefab(g); }, t));
            return g;
        }

        GameObject GetAvailableGO(PooledPrefabInfo info)
        {
            for (x = 0; x < info.AllObjects.Length; x++)
            {
                if (!info.AllObjects[x].InUse)
                {
                    info.AllObjects[x].InUse = true;
                    return info.AllObjects[x].Instance;
                }
            }

            return null;
        }

        public bool DeactivatePrefab(GameObject instance)
        {

            //instace.transform.SetParent(null);
            //UnityEngine.Object.DontDestroyOnLoad(instace);

            foreach(var a in prefabs.Values)
            {
                foreach(var b in a.AllObjects)
                {
                    if(b.Instance == instance && b.Instance.GetInstanceID() == instance.GetInstanceID())
                    {
                        var info = prefabs.Values.First(x => x.AllObjects.Where(y => y.Instance == instance).Count() != 0);
                        SetParent(instance, info);
                        instances[instance].InUse = false;

                        return true;
                    }
                }
            }

            /*
            foreach (var x in instaces.Keys)
            {
                if(instaces[x].Instance == instace || x == instace)
                {
                    SetParent(instace, pPrefabInfo);
                    instaces[instace].InUse = false;

                    return true;
                }
            }
            */


            foreach(var x in instances.Values)
            {
                if(x.Instance == null)
                {
                    Debug.LogError("Game object is null.. FUUUUCK!");

                }
                
            }

            Debug.LogErrorFormat("Cant deactivate Gameobject called {0}! Not present in 'instances'.", instance.name);
            return false;
        }

        /*
        PooledPrefabInfo GetAndAdd(GameObject prefab)
        {
            pPrefabs = prefabs.Values.ToArray();
            for (x = 0; x < pPrefabs.Length; x++)
            {
                for (y = 0; y < pPrefabs[x].AllObjects.Length; y++)
                {
                    Debug.LogFormat("{0}.{1} == {2}", pPrefabs[x].Name, pPrefabs[x].AllObjects[y].name, prefab.name);
                    if (pPrefabs[x].AllObjects[y] == prefab)
                    {
                        Debug.LogFormat("{0}.{1} == {2}", pPrefabs[x].Name, pPrefabs[x].AllObjects[y].name, prefab.name);
                        for (z = 0; z < pPrefabs[x].AvailableObjects.Length; z++)
                        {
                            if (pPrefabs[x].AvailableObjects[z] == null)
                                pPrefabs[x].AvailableObjects[z] = prefab;
                        }
                        return pPrefabs[x];
                    }
                }
            }

            return null;
        }
        */

        void SetParent(GameObject go, PooledPrefabInfo prefab, bool deactivate = true)
        {
            //Debug.Log(prefab.Name);
            GameObject g = GetOrCreateGO(prefabRepository, prefab.Type.ToString());
            go.transform.SetParent(g.transform);

            if (deactivate)
                go.SetActive(false);
        }
        GameObject GetOrCreateGO(Transform root, string name)
        {
            t = root.Find(name);
            if (t != null)
                return t.gameObject;
            else
            {
                GameObject g = new GameObject(name);
                g.transform.parent = root;
                return g;
            }
        }

        public void ForceDisable()
        {
            foreach (var prefs in prefabs.Values)
            {
                foreach (var o in prefs.AllObjects.Where(x => x.InUse))
                {
                    DeactivatePrefab(o.Instance);
                }
            }
        }

        List<PooledPrefab> tempGOs = new List<PooledPrefab>();
        PooledPrefab pPrefab;
        GameObject AddNew(PooledPrefabInfo data)
        {
            GameObject g = UnityEngine.Object.Instantiate(data.Prefab);
            UnityEngine.Object.DontDestroyOnLoad(g);

            pPrefab = new PooledPrefab() { Instance = g };
            tempGOs.Add(pPrefab);
            instances.Add(g, pPrefab);
            return g;
        }
    }
}
