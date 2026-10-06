// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using HarmonyLib;
using UnityEngine;

namespace SynesthesiaDev.DressmakerBadApple.Patches;

[HarmonyPatch(typeof(PatternPanel), nameof(PatternPanel.ShowRepresentationMannequin))]
public static class PatternPanelShowRepresentationMannequinPatch
{
    [HarmonyPostfix]
    public static void Postfix(PatternPanel instance)
    {
        if (BadAppleManager.BadAppleMaterial == null) return;
        if (instance.MannequinObj == null) return;

        var renderer = instance.MannequinObj.GetComponent<MeshRenderer>();

        if (renderer != null)
            renderer.material = BadAppleManager.BadAppleMaterial;
    }
}

