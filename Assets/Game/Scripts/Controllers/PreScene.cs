using UnityEngine;
using System.Collections;

public class PreScene : MonoBehaviour
{
    [SerializeField] bool AutoSwitchScene;

    private void Start()
    { 
        if (Application.loadedLevel == 0 && AutoSwitchScene)
                AppManager.Instance.LoadLevel(Application.loadedLevel + 1);
    }
}
