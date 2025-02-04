﻿using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class HoldableExporter : EditorWindow
{
    private HoldableDescriptor[] descriptorNotes;
    [MenuItem("Custom Cosmetics/Holdable Exporter")]

    public static void ShowWindow()
    {
        GetWindow(typeof(HoldableExporter), false, "Holdable Exporter", false);
    }

    public void OnFocus()
    {
        descriptorNotes = FindObjectsOfType<HoldableDescriptor>();
    }

    public Vector2 scrollPosition = Vector2.zero;
    public void OnGUI()
    {
        var window = GetWindow(typeof(HoldableExporter), false, "Holdable Exporter", false);

        int ScrollSpace = (16 + 20) + (16 + 17 + 17 + 20 + 20);
        foreach (var note in descriptorNotes)
        {
            if (note != null)
            {
                ScrollSpace += (16 + 17 + 17 + 20 + 20);
            }
        }

        float currentWindowWidth = EditorGUIUtility.currentViewWidth;
        float windowWidthIncludingScrollbar = currentWindowWidth;
        if (window.position.size.y >= ScrollSpace)
        {
            windowWidthIncludingScrollbar += 30;
        }

        scrollPosition = GUI.BeginScrollView(new Rect(0, 0, EditorGUIUtility.currentViewWidth, window.position.size.y), scrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - 20, ScrollSpace), false, false);

        foreach (HoldableDescriptor descriptorNote in descriptorNotes)
        {
            if (descriptorNote != null)
            {
                GUILayout.Label(descriptorNote.gameObject.name, EditorStyles.boldLabel, GUILayout.Height(16));
                descriptorNote.Name = EditorGUILayout.TextField("Name:", descriptorNote.Name, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(17));
                descriptorNote.Author = EditorGUILayout.TextField("Author:", descriptorNote.Author, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(17));
                descriptorNote.Description = EditorGUILayout.TextField("Description:", descriptorNote.Description, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(17));

                if (GUILayout.Button("Export " + descriptorNote.Name, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(20)))
                {
                    GameObject noteObject = descriptorNote.gameObject;
                    if (noteObject != null && descriptorNote != null)
                    {
                        if (descriptorNote.Name == "" || descriptorNote.Author == "" || descriptorNote.Description == "")
                        {
                            EditorUtility.DisplayDialog("Export Failed", "It is required to fill in the Name, Author, and Description for your holdable.", "OK");
                            return;
                        }

                        if (descriptorNote.gunEnabled)
                        {
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                            if (descriptorNote.shootSound == null || descriptorNote.bulletObject == null || descriptorNote.bulletSpeed == null || descriptorNote.bulletCooldown == null)
                            {
                                EditorUtility.DisplayDialog("Export Failed", "If your holdable is under the Gun module, it is required to fill in the Bullet Speed, Cooldown, Object, and Sound.", "OK");
                                return;
                            }
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'

#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                            if (descriptorNote.vibra == true && descriptorNote.strenth == null || descriptorNote.sTime == null)
                            {
                                EditorUtility.DisplayDialog("Export Failed", "If your holdable is under the Gun module and has vibrations, it is required to fill in the Strength and Duration.", "OK");
                                return;
                            }

                            if (descriptorNote.vibra && (descriptorNote.strenth > 0.5f || descriptorNote.sTime > 0.1f))
                            {
                                EditorUtility.DisplayDialog("Export Failed", "If your holdable is under the Gun module and has vibrations, the Strength has to be under 0.5 and Duration has to be under 0.1.", "OK");
                                return;
                            }
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                        }

                        string path = EditorUtility.SaveFilePanel("Where will you build your holdable?", "", descriptorNote.Name + ".holdable", "Holdable");

                        if (path != "")
                        {
                            Debug.ClearDeveloperConsole();
                            Debug.Log("Exporting holdable");
                            EditorUtility.SetDirty(descriptorNote);
                            BuildAssetBundle(descriptorNote.gameObject, path);
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Export Failed", "Please include the path to where the holdable will be exported at.", "OK");
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Export Failed", "The holdable object couldn't be found.", "OK");
                    }
                }
                GUILayout.Space(20);
            }
        }
        GUI.EndScrollView();
    }
    static public void BuildAssetBundle(GameObject obj, string path)
    {
        GameObject selectedObject = obj;
        string assetBundleDirectoryTEMP = "Assets/ExportedHoldables";

        HoldableDescriptor descriptor = selectedObject.GetComponent<HoldableDescriptor>();

        if (!AssetDatabase.IsValidFolder("Assets/ExportedHoldables"))
        {
            AssetDatabase.CreateFolder("Assets", "ExportedHoldables");
        }

        string holdableName = descriptor.Name;
        if(descriptor.leftHandObject == null && descriptor.rightHandObject == null)
        {
            EditorUtility.DisplayDialog("Export Failed", "Please make sure you have a reference to left and right hand holdable objects", "OK");
            return;
        }
        //string holdableAuthor = descriptor.Author;
        //string holdableDescription = descriptor.Description;
        //string holdableCustomColour = descriptor.customColours.ToString();

        string prefabPathTEMP = "Assets/ExportedHoldables/holdABLE.prefab";

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        PrefabUtility.SaveAsPrefabAsset(selectedObject.gameObject, prefabPathTEMP);
        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(prefabPathTEMP);
        contentsRoot.name = "holdABLE";

        // Text player_info = contentsRoot.AddComponent<Text>();
        // string split = "$"; // splits each of the strings
        // player_info.text = holdableName + split + holdableAuthor + split + holdableDescription + split + holdableLHand + split + holdableCustomColour;

        if (File.Exists(prefabPathTEMP))
        {
            File.Delete(prefabPathTEMP);
        }

        string newprefabPath = "Assets/ExportedHoldables/" + contentsRoot.name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(contentsRoot, newprefabPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);
        AssetImporter.GetAtPath(newprefabPath).SetAssetBundleNameAndVariant("holdableAssetBundle", "");

        if (!Directory.Exists("Assets/ExportedHoldables"))
        {
            Directory.CreateDirectory(assetBundleDirectoryTEMP);
        }

        string asset_new = assetBundleDirectoryTEMP + "/" + holdableName;
        if (File.Exists(asset_new + ".holdable"))
        {
            File.Delete(asset_new + ".holdable");
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectoryTEMP, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        if (File.Exists(newprefabPath))
        {
            File.Delete(newprefabPath);
        }

        string asset_temporary = assetBundleDirectoryTEMP + "/holdableAssetBundle";
        string metafile = asset_temporary + ".meta";
        if (File.Exists(asset_temporary))
        {
            File.Move(asset_temporary, asset_new + ".holdable");
        }

        AssetDatabase.Refresh();
        Debug.ClearDeveloperConsole();

        string path1 = assetBundleDirectoryTEMP + "/" + holdableName + ".holdable";
        string path2 = path;

        if (!File.Exists(path2)) // add
        {
            File.Move(path1, path2);
        }
        else // replace
        {
            File.Delete(path2);
            File.Move(path1, path2);
        }
        EditorUtility.DisplayDialog("Export Success", $"Your holdable was exported!", "OK");

        try
        {
            AssetDatabase.RemoveAssetBundleName("holdableassetbundle", true);
        }
        catch
        {

        }

        string holdablePath = path + "/";
        EditorUtility.RevealInFinder(holdablePath);

        if (AssetDatabase.IsValidFolder("Assets/ExportedHoldables"))
        {
            AssetDatabase.DeleteAsset("Assets/ExportedHoldables");
        }
    }
}