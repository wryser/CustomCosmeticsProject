using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class BadgeExporter : EditorWindow
{
    private BadgeDescriptor[] descriptorNotes;
    [MenuItem("Custom Cosmetics/Badge Exporter")]

    public static void ShowWindow()
    {
        GetWindow(typeof(BadgeExporter), false, "Badge Exporter", false);
    }

    public void OnFocus()
    {
        descriptorNotes = FindObjectsOfType<BadgeDescriptor>();
    }

    public Vector2 scrollPosition = Vector2.zero;
    public void OnGUI()
    {
        var window = GetWindow(typeof(BadgeExporter), false, "Badge Exporter", false);

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

        foreach (BadgeDescriptor descriptorNote in descriptorNotes)
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
                            EditorUtility.DisplayDialog("Export Failed", "It is required to fill in the Name, Author, and Description for your Badge.", "OK");
                            return;
                        }

                        string path = EditorUtility.SaveFilePanel("Where will you build your Badge?", "", descriptorNote.Name + ".badge", "Badge");

                        if (path != "")
                        {
                            Debug.ClearDeveloperConsole();
                            Debug.Log("Exporting Badge");
                            EditorUtility.SetDirty(descriptorNote);
                            BuildAssetBundle(descriptorNote.gameObject, path);
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Export Failed", "Please include the path to where the Badge will be exported at.", "OK");
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Export Failed", "The Badge object couldn't be found.", "OK");
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
        string assetBundleDirectoryTEMP = "Assets/ExportedBadges";

        BadgeDescriptor descriptor = selectedObject.GetComponent<BadgeDescriptor>();

        if (!AssetDatabase.IsValidFolder("Assets/ExportedBadges"))
        {
            AssetDatabase.CreateFolder("Assets", "ExportedBadges");
        }

        string BadgeName = descriptor.Name;
        // string BadgeAuthor = descriptor.Author;
        // string BadgeDescription = descriptor.Description;

        string prefabPathTEMP = "Assets/ExportedBadges/badge.prefab";

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        PrefabUtility.SaveAsPrefabAsset(selectedObject.gameObject, prefabPathTEMP);
        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(prefabPathTEMP);
        contentsRoot.name = "badge";

        // Text player_info = contentsRoot.AddComponent<Text>();
        // string split = "$"; // splits each of the strings
        // player_info.text = BadgeName + split + BadgeAuthor + split + BadgeDescription;

        if (File.Exists(prefabPathTEMP))
        {
            File.Delete(prefabPathTEMP);
        }

        string newprefabPath = "Assets/ExportedBadges/" + contentsRoot.name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(contentsRoot, newprefabPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);
        AssetImporter.GetAtPath(newprefabPath).SetAssetBundleNameAndVariant("BadgeAssetBundle", "");

        if (!Directory.Exists("Assets/ExportedBadges"))
        {
            Directory.CreateDirectory(assetBundleDirectoryTEMP);
        }

        string asset_new = assetBundleDirectoryTEMP + "/" + BadgeName;
        if (File.Exists(asset_new + ".badge"))
        {
            File.Delete(asset_new + ".badge");
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectoryTEMP, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        if (File.Exists(newprefabPath))
        {
            File.Delete(newprefabPath);
        }

        string asset_temporary = assetBundleDirectoryTEMP + "/BadgeAssetBundle";
        string metafile = asset_temporary + ".meta";
        if (File.Exists(asset_temporary))
        {
            File.Move(asset_temporary, asset_new + ".badge");
        }

        AssetDatabase.Refresh();
        Debug.ClearDeveloperConsole();

        string path1 = assetBundleDirectoryTEMP + "/" + BadgeName + ".badge";
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
        EditorUtility.DisplayDialog("Export Success", $"Your Badge was exported!", "OK");

        try
        {
            AssetDatabase.RemoveAssetBundleName("Badgeassetbundle", true);
        }
        catch
        {

        }

        string BadgePath = path + "/";
        EditorUtility.RevealInFinder(BadgePath);

        if (AssetDatabase.IsValidFolder("Assets/ExportedBadges"))
        {
            AssetDatabase.DeleteAsset("Assets/ExportedBadges");
        }
    }
}