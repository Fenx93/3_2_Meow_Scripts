﻿
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TextLocaliserEditWindow : EditorWindow
{
    public static void Open(string key)
    {
        TextLocaliserEditWindow window = new TextLocaliserEditWindow();
        window.titleContent = new GUIContent("Localiser Window");
        window.ShowUtility();
        window.key = key;

    }

    public string key, value;

    public void OnGUI()
    {
        key = EditorGUILayout.TextField("Key :", key);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Value:", GUILayout.MaxWidth(50));

        EditorStyles.textArea.wordWrap = true;
        value = EditorGUILayout.TextArea(value, EditorStyles.textArea, GUILayout.Height(100), GUILayout.Width(400));
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Add"))
        {
            if (LocalisationSystem.GetLocalisedValue(key) != string.Empty)
            {
                LocalisationSystem.Replace(key, value);
            }
            else
            {
                LocalisationSystem.Add(key, value);
            }
        }

        minSize = new Vector2(460, 250);
        maxSize = minSize;
    }
}

public class TextLocaliserSearchWindow: EditorWindow
{
    public static void Open()
    {
        TextLocaliserSearchWindow window = new TextLocaliserSearchWindow();
        window.titleContent = new GUIContent("Localisation Search");

        Vector2 mouse = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        Rect r = new Rect(mouse.x - 450, mouse.y + 10, 10, 10);
        window.ShowAsDropDown(r, new Vector2(500, 300));
    }
    public string value;
    public Vector2 scroll;
    public Dictionary<string, string> dictionary;

    private void OnEnable()
    {
        dictionary = LocalisationSystem.GetDictionaryForEditor();
    }

    public void OnGUI()
    {
        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField("Search: ", EditorStyles.boldLabel);
        value = EditorGUILayout.TextField(value);
        EditorGUILayout.EndHorizontal();

        GetSearchResults();
    }

    private void GetSearchResults()
    {
        if (value == null)
            return;

        EditorGUILayout.BeginVertical();
        scroll = EditorGUILayout.BeginScrollView(scroll);
        foreach (KeyValuePair<string,string> element in dictionary)
        {
            if (element.Key.ToLower().Contains(value.ToLower()) || element.Value.ToLower().Contains(value.ToLower()))
            {
                EditorGUILayout.BeginHorizontal("box");
                Texture closeIcon = (Texture)Resources.Load("close");

                GUIContent content = new GUIContent(closeIcon);

                if (GUILayout.Button(content, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                {
                    if (EditorUtility.DisplayDialog("Remove Key "+ element.Key + "?", " This will remove the element from localisation, are you sure?", "Do it"))
                    {
                        LocalisationSystem.Remove(element.Key, element.Value);
                        AssetDatabase.Refresh();
                        LocalisationSystem.Init();
                        dictionary = LocalisationSystem.GetDictionaryForEditor();
                    }
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();

            }
        }
    }
}

