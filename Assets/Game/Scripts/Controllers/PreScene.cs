using UnityEngine;
using System.Collections;
using US = UnityEngine.SceneManagement;
namespace SteelSkies
{
    public class PreScene : MonoBehaviour
    {
        [SerializeField] bool autoSwitchScene;
        [SerializeField] bool pressAnyKey;

        private void Start()
        {
            if (autoSwitchScene)
            {
                US.SceneManager.LoadScene(US.SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
        private void Update()
        {
            if (pressAnyKey)
            {
                if (Input.anyKeyDown)
                    US.SceneManager.LoadScene(US.SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}