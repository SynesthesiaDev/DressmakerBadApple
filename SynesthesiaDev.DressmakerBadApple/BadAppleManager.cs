// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using BepInEx;
using UnityEngine;
using System.IO;

namespace SynesthesiaDev.DressmakerBadApple;

public static class BadAppleManager
{
    public static Material BadAppleMaterial;
    public static Texture2D SpriteSheet;
    public static Texture2D[] Frames;
    public static int TotalFrames;

    public static int CurrentFrame;
    public static float FrameTimer;
    public static bool IsPlaying = false;

    public const int COLUMNS = 72;
    public const int ROWS = 72;
    public const int FRAME_WIDTH = 192;
    public const int FRAME_HEIGHT = 192;
    public const float FRAME_RATE = 24f;

    private static int lastFrame = -1;
    private const float frame_duration = 1f / FRAME_RATE;

    public static void AdvanceFrame()
    {
        if (!IsPlaying) return;
        if (Time.frameCount == lastFrame) return;
        lastFrame = Time.frameCount;

        FrameTimer += Time.deltaTime;
        if (!(FrameTimer >= frame_duration)) return;

        FrameTimer -= frame_duration;
        CurrentFrame++;

        if (CurrentFrame >= TotalFrames)
            CurrentFrame = 0;
    }

    public static void Initialize()
    {
        if (BadAppleMaterial != null) return;

        var spriteSheetPath = Path.Combine(Paths.PluginPath, "DressmakerBadApple", "badapple_spritesheet.png");
        if (!File.Exists(spriteSheetPath))
        {
            Plugin.Log.LogError($"spritesheet no exist! ({spriteSheetPath})");
            return;
        }

        var fileData = File.ReadAllBytes(spriteSheetPath);
        SpriteSheet = new Texture2D(2, 2);
        SpriteSheet.LoadImage(fileData);

        TotalFrames = COLUMNS * ROWS;
        Frames = new Texture2D[TotalFrames];

        for (var i = 0; i < TotalFrames; i++)
        {
            var col = i % COLUMNS;
            var row = i / COLUMNS;

            // unity texture Y is bottom-up so we flip the row index
            var x = col * FRAME_WIDTH;
            var y = (ROWS - 1 - row) * FRAME_HEIGHT;

            var tex = new Texture2D(FRAME_WIDTH, FRAME_HEIGHT, TextureFormat.RGBA32, false);
            tex.SetPixels(SpriteSheet.GetPixels(x, y, FRAME_WIDTH, FRAME_HEIGHT));
            tex.Apply();
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Point;
            Frames[i] = tex;
            Frames[i] = tex;
        }

        var unlitShader = Shader.Find("Universal Render Pipeline/Unlit")
                          ?? Shader.Find("Sprites/Default")
                          ?? Shader.Find("Unlit/Texture");

        if (unlitShader == null)
        {
            Plugin.Log.LogError("No usable unlit shader found");
            return;
        }

        BadAppleMaterial = new Material(unlitShader);
        BadAppleMaterial.SetTexture("_BaseMap", Frames[0]);
    }


    // Rebake UVs so it looks properly stretched over the whole dress instead of
    // on each part individually
    public static void RebakeUVs(PatternPanel panel)
    {
        var meshFilter = panel.MannequinMeshFilter;
        if (meshFilter == null) return;

        var mesh = meshFilter.sharedMesh;
        if (mesh == null) return;

        var mannequin = panel.AssignedMannequin;
        if (mannequin == null) return;

        // value! fresh out of my ass
        // (offsetting the projected texture so it looks nice and centered)
        var dressMin = new Vector2(-0.35f, 0.65f);
        var dressMax = new Vector2(0.35f, 1.45f);

        var width = dressMax.x - dressMin.x;
        var height = dressMax.y - dressMin.y;

        var vertices = mesh.vertices;
        var uvs = new Vector2[vertices.Length];
        var t = mannequin.transform;

        for (var i = 0; i < vertices.Length; i++)
        {
            var worldPos = meshFilter.transform.TransformPoint(vertices[i]);
            var localPos = t.InverseTransformPoint(worldPos);
            uvs[i] = new Vector2(
                Mathf.Clamp01((localPos.x - dressMin.x) / width),
                Mathf.Clamp01((localPos.y - dressMin.y) / height)
            );
        }

        mesh.SetUVs(0, uvs);
        mesh.UploadMeshData(false);
    }
}
