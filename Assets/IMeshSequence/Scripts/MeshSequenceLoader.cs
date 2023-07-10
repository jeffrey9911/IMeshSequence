using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSequenceLoader : MonoBehaviour
{
    public string FolderName;
    public string FirstMeshName;
    public int Frames;

    private string FolderBasePath = "IMeshSequence\\";

    public List<GameObject> ObjectSequence = new List<GameObject>();

    [ContextMenu("LoadMeshSequence")]
    public void LoadMeshSequence()
    {
        int IndexDigits = 0;

        for(int i = FirstMeshName.Length - 1; i >= 0; i--)
        {
            if (char.IsDigit(FirstMeshName[i])) IndexDigits++;
            else break;
        }

        string MeshName = FirstMeshName.Substring(0, FirstMeshName.Length - IndexDigits);

        FolderBasePath += $"{FolderName}\\{MeshName}";

        Debug.Log($"Mesh Name Loaded: {MeshName}, Index format: {(0).ToString($"D{IndexDigits}")}");

        for(int i = 0; i < Frames; i++)
        {
            ObjectSequence.Add(Instantiate(Resources.Load<GameObject>(FolderBasePath + i.ToString($"D{IndexDigits}")), this.transform));
        }

        for(int i = 0; i < ObjectSequence.Count; i++)
        {
            if (i != 0) ObjectSequence[i].SetActive(false);
        }
    }

    public void DeactiveAll()
    {
        foreach(GameObject obj in ObjectSequence)
        {
            obj.SetActive(false);
        }
    }

}
