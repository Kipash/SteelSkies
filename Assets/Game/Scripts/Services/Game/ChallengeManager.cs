﻿using UnityEngine;
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
    public PathSettings PathSettings;
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

    [Header("Combos & bonuses")]
    [SerializeField] float minimalKillDelay;

    [Header("Debug")]
    [SerializeField] bool disabled;

    List<GameObject> currentGameObjects = new List<GameObject>();

    bool isSpawningWave;

    public const string SpawningTag = "SpawningTag";


    float lastKill;
    int currCombo;

    int score;
    public int Score
    {
        get { return score; }
        set { score = value; GameServices.Instance.GameUIManager.GameScore.SetNumericDial(Score); }
    }

    //public int BestScore;

    public void Initialize()
    {
        Timing.Instance.AddTag(SpawningTag, false);

        GameServices.Instance.GameManager.OnGameStart += ScanOponents;
        GameServices.Instance.GameManager.OnGameOver += StopAndDestroyCurrentWave;

        Score = 0;
    }

    public void DeactivateEntity(GameObject go, bool natural)
    {
        if (currentGameObjects.Contains(go))
        {
            if(natural)
            {
                int s = 10;

                //Debug.LogFormat("{0} - {1} < {2}", Time.time, lastKill, minimalKillDelay);
                if (Time.time - lastKill < minimalKillDelay)
                {
                    currCombo++;
                }
                else
                    currCombo = 0;

                var msg = currCombo > 0 ? string.Format("+{0}X{1}", s, currCombo + 1) : string.Format("+{0}",s);
                GameServices.Instance.GameUIManager.ShowDialog(go.transform.position, msg);
                Score += s * (currCombo + 1);

                lastKill = Time.time;
            }

            AppServices.Instance.PoolManager.DeactivatePrefab(go);
            currentGameObjects.Remove(go);
            ScanOponents();
        }
        else
            Debug.LogErrorFormat("Game object '{0}' is not listed!", go.name);
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

        var enemy = pref.GetComponentInChildren<EnemyComponent>();
        enemy.PathSettings = info.PathSettings;
        
        currentGameObjects.Add(pref);
    }

    void ScanOponents()
    {
        if(currentGameObjects.Count <= 0 && !isSpawningWave)
        {
            isSpawningWave = true;

            var wave = waves[UnityEngine.Random.Range(0, waves.Count)];
            Timing.RunCoroutine(SpawnWave(wave), SpawningTag);
        }
    }

    void StopAndDestroyCurrentWave()
    {
        Timing.Instance.KillCoroutinesOnInstance(SpawningTag);

        foreach (var x in currentGameObjects.ToArray())
            DeactivateEntity(x, false);

        Score = 0;
        isSpawningWave = false;
    }

    IEnumerator<float> SpawnWave(WaveInfo info)
    {
        yield return Timing.WaitForSeconds(info.PreWarm);
        AppServices.Instance.AudioManager.SoundEffectsManager.PlaySound(SoundEffects.Warning);

        var max = info.Wave.Count;
        
        for (int i = 0; i < max; i++)
        {
            SpawnEnemy(info.Wave[i]);

            if (i + 1 == info.Wave.Count)
                isSpawningWave = false;
            else
                yield return Timing.WaitForSeconds(info.Wave[i].SpawnDelay);
        }
    }
}
