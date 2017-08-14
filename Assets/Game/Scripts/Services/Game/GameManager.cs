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
        Debug.Log("ResetLevel");
    }
    public void ResetLevel()
    {
        Timing.RunCoroutine(ResetLevelDelayed(0));
        Debug.Log("ResetLevel");

    }

    IEnumerator<float> ResetLevelDelayed(float delay)
    {
        yield return Timing.WaitForSeconds(delay);
        Debug.Log("fml");
        AppManager.Instance.LoadLevel(Application.loadedLevel);
    }
}
