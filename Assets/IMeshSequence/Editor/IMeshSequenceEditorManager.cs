using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshSequenceLoader))]
public class IMeshSequenceLoaderEditorManager : Editor
{

    SerializedProperty MeshSequenceFolder;
    MeshSequenceLoader meshSequenceLoader;

    private void OnEnable()
    {
        MeshSequenceFolder = serializedObject.FindProperty("MeshSequenceFolder");

        meshSequenceLoader = (MeshSequenceLoader)target;

        serializedObject.ApplyModifiedProperties();
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        if (GUILayout.Button("Load Sequence Folder Path"))
        {
            string path = EditorUtility.OpenFolderPanel("Open a folder that contains a Geometry Sequence (.obj)", meshSequenceLoader.MeshSequenceFolder, "");
            MeshSequenceFolder.stringValue = path;
        }

        if (GUILayout.Button("Load Sequenced Mesh"))
        {
            meshSequenceLoader.LoadMeshSequenceByPath();
        }

        if(meshSequenceLoader != null)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}


[CustomEditor(typeof(MeshSequencePlayer))]
public class IMeshSequencePlayerEditorManager : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        MeshSequencePlayer meshSequencePlayer = (MeshSequencePlayer)target;

        if (meshSequencePlayer.isPlayingAudio)
        {
            SetPropertyField("PlayerAudio", meshSequencePlayer.isPlayingAudio);
        }

        serializedObject.ApplyModifiedProperties();
    }

    void SetPropertyField(string name, bool set)
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty(name), set);
    }
}

public class HierarchyContextMenu : Editor
{
    [MenuItem("GameObject/Mesh Sequence Loader", false, 10)]
    static void InstantiateSequenceLoader()
    {
        string prefabPath = "Assets/IMeshSequence/Prefab/MeshSequence.prefab";
        GameObject MeshSequenceLoaderPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (MeshSequenceLoaderPrefab != null)
        {
            PrefabUtility.InstantiatePrefab(MeshSequenceLoaderPrefab);
        }
    }
}