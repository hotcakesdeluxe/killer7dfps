using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Texto
{
    public class TextoBrowserWindow : EditorWindow
    {
        [MenuItem("Window/PHL/Texto Browser")]
        private static void OpenWindow()
        {
            Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/PHLCommon/Logster/Editor/Images/LogsterIcon.png");
            TextoBrowserWindow window = GetWindow<TextoBrowserWindow>();
            window.titleContent = new GUIContent("Texto", icon);
            window.Show();
        }

        private void OnEnable()
        {

        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Coming soon!");
        }
    }
}