using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;

public enum LocationType { none = 0, Relative = 1,  World = 2}

[Serializable]
public class OpponentInfo
{
    public string Name;
    public LocationType Location;
    public PooledPrefabs Prefab;
    public Vector3 Offset;
    public float SpawnDelay;
}

[Serializable]
public class WaveInfo
{
    public float PreWarm;
    public List<OpponentInfo> Wave;
}

[Serializable]
public class ChallengeManager
{
    [Header("Roots")]
    [SerializeField] Transform relativeRoot;
    [SerializeField] Transform WorldRoot;

    [Header("Wawe")]
    [SerializeField] List<WaveInfo> waves;

    [Header("Debug")]
    [SerializeField] bool disabled;

    List<GameObject> currentGameObjects = new List<GameObject>();

    bool isSpawningWave;

    public int Score;
    public int BestScore;

    public void Start()
    {
        ScanOponents();

        var score = PlayerPrefs.GetInt("score", -1);
        if (score != -1)
            BestScore = score;
        else
            BestScore = 0;
    }

    public void DeactivateEntity(GameObject go)
    {
        if (currentGameObjects.Contains(go))
        {
            AppServices.Instance.PoolManager.DeactivatePrefab(go);
            currentGameObjects.Remove(go);
            ScanOponents();

            Score += 10;
            var score = PlayerPrefs.GetInt("score", -1);
            if (score != -1)
            {
                if (score > Score)
                    BestScore = score;
                else
                    BestScore = Score;
            }

            PlayerPrefs.SetInt("score", Score);
        }
        else
            UnityEngine.Object.Destroy(go);
    }
    
    void SpawnEnemy(OpponentInfo info)
    {
        if (disabled)
            return;
        var pref = AppServices.Instance.PoolManager.GetPooledPrefab(info.Prefab);

        if (info.Location == LocationType.Relative)
        {
            pref.transform.parent = relativeRoot;
            pref.transform.localPosition = info.Offset;
        }
        else if(info.Location == LocationType.World)
        {
            pref.transform.parent = WorldRoot;
            pref.transform.localPosition = relativeRoot.position + info.Offset;
        }

        currentGameObjects.Add(pref);
    }

    void ScanOponents()
    {
        if(currentGameObjects.Count <= 0 && !isSpawningWave)
        {
            isSpawningWave = true;

            var wave = waves[UnityEngine.Random.Range(0, waves.Count)];
            Timing.RunCoroutine(SpawnWave(wave));
        }
    }
    IEnumerator<float> SpawnWave(WaveInfo info)
    {
        yield return Timing.WaitForSeconds(info.PreWarm);
        AppServices.Instance.AudioManager.SoundEffectsManager.PlaySound(SoundEffects.Warning);

        for (int i = 0; i < info.Wave.Count; i++)
        {
            SpawnEnemy(info.Wave[i]);
            if (i + 1 == info.Wave.Count)
                isSpawningWave = false;
            else
                yield return Timing.WaitForSeconds(info.Wave[i].SpawnDelay);
        }
    }
}
