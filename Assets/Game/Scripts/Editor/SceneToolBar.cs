using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;

namespace SteelSkies
{
    public class SceneToolBar : MonoBehaviour
    {
        [MenuItem("Tools/Scenes/Pre scene")]
        public static void LoadPreScene()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Game/Scenes/PreScene.unity");
        }
        [MenuItem("Tools/Scenes/Menu")]
        public static void LoadMenu()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Game/Scenes/Menu.unity");
        }
        [MenuItem("Tools/Scenes/Main")]
        public static void LoadLevel()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Game/Scenes/Main.unity");
        }
    }
}