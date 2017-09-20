using AppBackend;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AppBackend
{
    [CustomEditor(typeof(FileConfig))]
    public class FileConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            FileConfig fileConfig = (FileConfig)target;

            if (GUILayout.Button("Delete UserData"))
            {
                fileConfig.DeleteUserData();
            }
        }
    }
}