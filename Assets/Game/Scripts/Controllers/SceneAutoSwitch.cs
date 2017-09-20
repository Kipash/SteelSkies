using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Aponi
{
    public class SceneAutoSwitch : MonoBehaviour
    {
        private void Awake()
        {
            if (!AppServices.Initiliazed)
                UnityEngine.SceneManagement.SceneManager.LoadScene(0, LoadSceneMode.Single);
            else
                Destroy(gameObject);
        }
    }
}