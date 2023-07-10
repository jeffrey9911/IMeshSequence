using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSequencePlayer : MonoBehaviour
{
    public int PlayerFramePerSecond = 30;

    public bool isPlaying = false;

    private int CurrentFrame = 0;

    private float FrameTimer = 0;

    private MeshSequenceLoader Loader;

    private void Start()
    {
        Loader = GetComponent<MeshSequenceLoader>();
        Loader.DeactiveAll();
    }

    private void Update()
    {
        if(isPlaying)
        {
            FrameTimer += Time.deltaTime;

            if(FrameTimer >= 1f/ PlayerFramePerSecond)
            {
                SwapFrame();
                FrameTimer = 0;
            }

        }
    }

    private void SwapFrame()
    {
        int NextFrame = (CurrentFrame + 1) >= Loader.Frames ? 0 : CurrentFrame + 1;

        Loader.ObjectSequence[CurrentFrame].SetActive(false);
        Loader.ObjectSequence[NextFrame].SetActive(true);

        CurrentFrame = NextFrame;
    }
}
