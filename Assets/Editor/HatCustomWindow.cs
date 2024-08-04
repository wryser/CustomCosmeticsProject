using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;

[CustomEditor(typeof(HatDescriptor))]
//https://blog.theknightsofunity.com/use-customeditor-customize-script-inspector/

public class HatCustomWindow : Editor
{
    public SerializedProperty Name;
    public SerializedProperty Author;
    public SerializedProperty Description;
    public SerializedProperty id;

    public SerializedProperty behaviours;
    private void OnEnable()
    {
        Name = serializedObject.FindProperty("Name");
        Author = serializedObject.FindProperty("Author");
        Description = serializedObject.FindProperty("Description");
        id = serializedObject.FindProperty("id");
        behaviours = serializedObject.FindProperty("behaviours");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(Name, new GUIContent("Hat Name"));
        EditorGUILayout.PropertyField(Author, new GUIContent("Hat Author"));
        EditorGUILayout.PropertyField(Description, new GUIContent("Hat Description"));
        EditorGUILayout.PropertyField(behaviours, new GUIContent("Cosmetic Behaviours"));

        //EditorGUI.
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }
}
