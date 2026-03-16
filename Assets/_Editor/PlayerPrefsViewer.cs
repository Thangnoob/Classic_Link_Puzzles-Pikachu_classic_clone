using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PlayerPrefsViewer : EditorWindow
{
    private Vector2 scroll;

    private List<string> keys = new List<string>()
    {
        "CurrentLevelIndexKey",
        "sfx_volume",
        "player_level",
        "high_score"
    };

    private string newKey = "";
    private string newValue = "";

    [MenuItem("Tools/PlayerPrefs Viewer")]
    public static void ShowWindow()
    {
        GetWindow<PlayerPrefsViewer>("PlayerPrefs Viewer");
    }

    void OnGUI()
    {
        GUILayout.Label("PlayerPrefs Debug Tool", EditorStyles.boldLabel);

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var key in keys)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(key, GUILayout.Width(150));

            string value = PlayerPrefs.GetString(key, "");
            string edited = EditorGUILayout.TextField(value);

            if (edited != value)
            {
                PlayerPrefs.SetString(key, edited);
                PlayerPrefs.Save();
            }

            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                PlayerPrefs.DeleteKey(key);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        GUILayout.Label("Add / Set Key", EditorStyles.boldLabel);

        newKey = EditorGUILayout.TextField("Key", newKey);
        newValue = EditorGUILayout.TextField("Value", newValue);

        if (GUILayout.Button("Set Value"))
        {
            if (!string.IsNullOrEmpty(newKey))
            {
                PlayerPrefs.SetString(newKey, newValue);
                PlayerPrefs.Save();

                if (!keys.Contains(newKey))
                    keys.Add(newKey);

                newKey = "";
                newValue = "";
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Delete All PlayerPrefs"))
        {
            if (EditorUtility.DisplayDialog(
                "Warning",
                "Delete all PlayerPrefs?",
                "Yes",
                "Cancel"))
            {
                PlayerPrefs.DeleteAll();
            }
        }
    }
}