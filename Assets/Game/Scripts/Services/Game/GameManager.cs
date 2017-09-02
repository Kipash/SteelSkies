using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;

[Serializable]
public class GameManager
{
    public void ResetLevel(float delay)
    {
        Timing.RunCoroutine(ResetLevelDelayed(delay));
    }
    public void ResetLevel()
    {
        Timing.RunCoroutine(ResetLevelDelayed(0));
    }

    IEnumerator<float> ResetLevelDelayed(float delay)
    {
        yield return Timing.WaitForSeconds(delay);  
        AppManager.Instance.LoadLevel(Application.loadedLevel);
    }
}
