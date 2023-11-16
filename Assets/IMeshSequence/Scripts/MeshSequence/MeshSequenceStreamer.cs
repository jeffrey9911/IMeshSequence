using Dummiesman;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class MeshSequenceStreamer : MonoBehaviour
{
    [SerializeField] public string MeshSequencePath;

    public float PlayerFramePerSecond = 30;

    public bool isPlaying = false;
    private bool isLoaded = false;

    private int CurrentFrameIndex = 0;

    private float FrameTimer = 0;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private MeshSequenceContainer meshSequenceContainer;
    
    private bool isPlayingAudio = false;
    private AudioSource PlayerAudioSource;

    private string[] FramePaths;
    private int FrameCount = 0;

    OBJLoader OBJLoader = new OBJLoader();

    private void Start()
    {

        //Debug.Log(Application.dataPath);
        //Debug.Log("D:\\_FILE_BROWSER_\\_FILE_STATION_\\230803-StudyVR\\Patrick\\talk1-full\\iMS-Patrick-talk-full-01\\QLOW\\talk1full_QLOW_0000000.obj");
    }

    private void Update()
    {
        if (isPlaying)
        {
            if(isPlayingAudio)
            {
                if(Mathf.FloorToInt(PlayerAudioSource.time / PlayerAudioSource.clip.length * (FrameCount - 1)) != CurrentFrameIndex)
                {
                    SwapFrame();
                }
            }
            else
            {
                FrameTimer += Time.deltaTime;

                if (FrameTimer >= (1 / PlayerFramePerSecond))
                {

                    if (isLoaded)
                    {
                        SwapFrame();
                    }

                    FrameTimer = 0;
                }
            }
        }
    }

    public void StartLoad()
    {
        LoadMeshSequenceInfo();
    }

    
    public void LoadMeshSequenceInfo(string folderPath = "", Action<float> ProgressCallback = null)
    {
        if (folderPath == "") { folderPath = MeshSequencePath; }

        Debug.Log(folderPath);

        if (Directory.Exists(folderPath))
        {
            FramePaths = Directory.GetFiles(folderPath, "*.obj");
            
            string[] AudioPath = Directory.GetFiles(folderPath, "*.wav");

            if (FramePaths.Length > 0)
            {
                FrameCount = FramePaths.Length;

                if(AudioPath.Length > 0)
                {
                    PlayerAudioSource = this.gameObject.AddComponent<AudioSource>();

                    StartCoroutine(LoadAudio(AudioPath[0]));
                }

                meshSequenceContainer = this.gameObject.AddComponent<MeshSequenceContainer>();
                meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
                meshFilter = this.gameObject.AddComponent<MeshFilter>();

                StartCoroutine(LoadFrames(ProgressCallback));
            }
        }
    }

    [ContextMenu("Play")]
    public void Play()
    {
        isPlaying = true;

        if(isPlayingAudio)
        {
            PlayerAudioSource.Play();
        }
    }

    public IEnumerator LoadAudio(string filePath)
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error loading audio file: " + www.error);
        }
        else
        {
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);

            if (audioClip != null)
            {
                Debug.Log("Audio clip loaded successfully.");
                PlayerAudioSource.clip = audioClip;
                PlayerAudioSource.loop = true;

                isPlayingAudio = true;
            }
            else
            {
                Debug.LogError("Failed to load audio clip from path: " + filePath);
            }
        }

        www.Dispose();
    }


    public IEnumerator LoadFrames(Action<float> ProgressCallback = null)
    {
        while(meshSequenceContainer.MeshSequence.Count < (CurrentFrameIndex + 1))
        {
            OBJLoader.LoadMatMesh(FramePaths[CurrentFrameIndex], MeshCallback, MatCallback);

            if (ProgressCallback != null) ProgressCallback.Invoke((1.0f * CurrentFrameIndex / FrameCount) * 100f);

            CurrentFrameIndex = (CurrentFrameIndex + 1) >= FrameCount ? 0 : CurrentFrameIndex + 1;

            yield return null;
        }

        isLoaded = true;
    }



    private void SwapFrame(bool isReversing = false)
    {
        meshRenderer.sharedMaterial = meshSequenceContainer.MaterialSequence[CurrentFrameIndex];
        meshFilter.mesh = meshSequenceContainer.MeshSequence[CurrentFrameIndex];

        int NextFrame = 0;
        if (isReversing)
        {
            NextFrame = (CurrentFrameIndex - 1) < 0 ? FrameCount - 1 : CurrentFrameIndex - 1;
        }
        else
        {
            NextFrame = (CurrentFrameIndex + 1) >= FrameCount ? 0 : CurrentFrameIndex + 1;
        }

        CurrentFrameIndex = NextFrame;
    }



    void MatCallback(Material[] mats)
    {
        meshSequenceContainer.MaterialSequence.Add(mats[0]);
    }

    void MeshCallback(Mesh msf)
    {
        meshSequenceContainer.MeshSequence.Add(msf);
    }
}
