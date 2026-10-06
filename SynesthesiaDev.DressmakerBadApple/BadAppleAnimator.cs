// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using UnityEngine;

namespace SynesthesiaDev.DressmakerBadApple;

public class BadAppleAnimator : MonoBehaviour
{
    private MeshRenderer renderer;
    private static int lastInputFrame = -1;

    protected void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    protected void Update()
    {
        // only one animator should handle per frame input no matter how many panels exist
        if (Time.frameCount != lastInputFrame)
        {
            lastInputFrame = Time.frameCount;
            handleInput();
        }

        if (BadAppleManager.BadAppleMaterial == null) return;
        if (BadAppleManager.Frames == null || BadAppleManager.Frames.Length == 0) return;
        if (renderer == null) renderer = GetComponent<MeshRenderer>();
        if (renderer == null) return;

        if (renderer.sharedMaterial != BadAppleManager.BadAppleMaterial)
            renderer.sharedMaterial = BadAppleManager.BadAppleMaterial;

        BadAppleManager.AdvanceFrame();
        renderer.material.SetTexture("_BaseMap", BadAppleManager.Frames[BadAppleManager.CurrentFrame]);
    }

    // debug input binds
    private static void handleInput()
    {
        // play/pause
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BadAppleManager.IsPlaying = !BadAppleManager.IsPlaying;
        }

        // reset
        if (Input.GetKeyDown(KeyCode.R))
        {
            BadAppleManager.CurrentFrame = 0;
            BadAppleManager.FrameTimer = 0f;
            BadAppleManager.IsPlaying = false;
        }

        // forward a frame
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            BadAppleManager.IsPlaying = false;
            BadAppleManager.CurrentFrame = Mathf.Min(BadAppleManager.CurrentFrame + 1, BadAppleManager.TotalFrames - 1);
        }

        // backwards a frame
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            BadAppleManager.IsPlaying = false;
            BadAppleManager.CurrentFrame = Mathf.Max(BadAppleManager.CurrentFrame - 1, 0);
        }
    }
}
