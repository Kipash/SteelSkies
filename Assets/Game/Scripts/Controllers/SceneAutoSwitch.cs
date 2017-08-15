using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAutoSwitch : MonoBehaviour {
    private void Awake()
    {
        if (GameObject.FindObjectOfType<AppManager>() == null)
            SceneManager.LoadScene(0, LoadSceneMode.Single);   
        else
            Destroy(gameObject);
    }
}