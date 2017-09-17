using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;

public class EditorTools : MonoBehaviour
{
    [MenuItem("Tools/Scenes/Main")]
    public static void LoadStadion()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Game/Scenes/Main.unity");
    }
    [MenuItem("Tools/Scenes/StartIntro")]
    public static void LoadIntro()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Game/Scenes/StartCinematic.unity");
    }
    [MenuItem("Tools/Scenes/Pre scene")]
    public static void LoadMainMenu()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Game/Scenes/PreScene.unity");
    }
}
