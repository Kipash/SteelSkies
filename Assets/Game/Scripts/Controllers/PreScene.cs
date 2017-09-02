using UnityEngine;
using System.Collections;
using MovementEffects;

public class PreScene : MonoBehaviour
{
    [SerializeField] bool AutoSwitchScene;

    private void Start()
    {
        if (Application.loadedLevel == 0 && AutoSwitchScene)
        {
            Timing.KillCoroutines();
            AppManager.Instance.LoadLevel(Application.loadedLevel + 1);
        }
    }
}
