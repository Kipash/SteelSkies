using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Aponi
{

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
        [SerializeField]
        Transform prefabRepository;
        [SerializeField] bool preBuild;

        Dictionary<PooledPrefabs, PooledPrefab> prefabs = new Dictionary<global::PooledPrefabs, PooledPrefab>();

        PooledPrefab[] pPrefabs;
        PooledPrefab pPrefab;
        GameObject g;
        Transform t;

        public void Initialize()
        {
            //Timing.Instance.AddTag(poolTag, true);
            prefabs = pooledPrefabs.GroupBy(x => x.Type).ToDictionary((x => x.Key), (x => x.First()));
            AppServices.Instance.SceneManager.OnSceneChanged += SceneManager_OnSceneChanged;

            if (preBuild)
                PreBuild();
        }

        private void SceneManager_OnSceneChanged(Scenes newScene, Scenes oldScene)
        {
            ForceDisable();
        }

        void PreBuild()
        {
            foreach (var p in pooledPrefabs)
            {
                for (int i = 0; i < pooledPrefabs.Length; i++)
                {
                    for (int y = 0; y < pooledPrefabs[i].PreBuild; y++)
                    {
                        g = AddNew(pooledPrefabs[i].Type);
                        SetParent(g, p);
                    }
                }
                
            }
        }

        public GameObject GetPooledPrefab(PooledPrefabs prefab)
        {
            if (!prefabs.ContainsKey(prefab))
                UnityEngine.Debug.LogErrorFormat("No PooledPrefab of type {0} is registered!", prefab);

            pPrefab = prefabs[prefab];

            //UnityEngine.Debug.Log(prefab);

            if (pPrefab.AvailableObjects.Count == 0)
            {
                g = AddNew(prefab);
            }
            else
            {
                g = pPrefab.AvailableObjects.First();
            }
            pPrefab.AvailableObjects.Remove(g);
            g.SetActive(true);
            g.transform.SetParent(null);

            return g;
        }
        public GameObject GetPooledPrefabTimed(PooledPrefabs prefab, float t)
        {
            g = GetPooledPrefab(prefab);
            AppServices.Instance.StartCoroutine(CommonCoroutine.CallDelay(() => { DeactivatePrefab(g); }, t));
            return g;
        }

        public bool DeactivatePrefab(GameObject prefab)
        {
            pPrefabs = prefabs.Values.Where(x => x.AllObjects.Contains(prefab)).ToArray();
            if (pPrefabs.Length != 0)
            {
                pPrefab = pPrefabs[0];

                SetParent(prefab, pPrefab);
                if (!pPrefab.AvailableObjects.Contains(prefab))
                    pPrefab.AvailableObjects.Add(prefab);

                return true;
            }
            else
                return false;
        }

        
        void SetParent(GameObject go, PooledPrefab prefab, bool deactivate = true)
        {
            g = GetOrCreateGO(prefabRepository, prefab.Type.ToString());
            go.transform.SetParent(g.transform);

            if (deactivate)
                go.SetActive(false);
        }
        GameObject AddNew(PooledPrefabs type)
        {
            pPrefab = prefabs[type];
            g = UnityEngine.Object.Instantiate(pPrefab.Prefab);
            pPrefab.AllObjects.Add(g);
            pPrefab.AvailableObjects.Add(g);
            return g;
        }
        GameObject GetOrCreateGO(Transform root, string name)
        {
            t = root.Find(name);
            if (t != null)
                return t.gameObject;
            else
            {
                g = new GameObject(name);
                g.transform.parent = root;
                return g;
            }
        }

        public void ForceDisable()
        {
            foreach (var prefs in prefabs.Values)
            {
                foreach (var o in prefs.AllObjects.Except(prefs.AvailableObjects))
                {
                    DeactivatePrefab(o);
                }
            }
        }
    }
}