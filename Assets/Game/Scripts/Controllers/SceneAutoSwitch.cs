using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAutoSwitch : MonoBehaviour {
    private void Awake()
    {
        if (GameObject.FindObjectOfType<AppManager>() == null)
            Application.LoadLevel(0);
        else
            Destroy(gameObject);
    }
}
