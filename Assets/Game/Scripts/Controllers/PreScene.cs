using UnityEngine;
using System.Collections;
using MovementEffects;

public class PreScene : MonoBehaviour
{
    [SerializeField] bool autoSwitchScene;
    [SerializeField] bool pressAnyKey;

    private void Start()
    {
        if (autoSwitchScene)
        {
            Timing.KillCoroutines();
            AppManager.Instance.LoadLevel(Application.loadedLevel + 1);
        }
    }
    private void Update()
    {
        if (pressAnyKey)
        {
            if (Input.anyKeyDown)
                AppManager.Instance.LoadLevel(Application.loadedLevel + 1);
        }

    }
}
