// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using HarmonyLib;
using UnityEngine;

namespace SynesthesiaDev.DressmakerBadApple.Patches;

[HarmonyPatch(typeof(PatternPanel), nameof(PatternPanel.CutOutWithMaterial))]
public static class PatternPanelCutOutWithMaterialPatch
{
    [HarmonyPostfix]
    public static void Postfix(PatternPanel instance)
    {
        if (BadAppleManager.BadAppleMaterial == null)
            BadAppleManager.Initialize();

        if (BadAppleManager.BadAppleMaterial == null) return;

        if (instance.MannequinObj != null)
        {
            var r = instance.MannequinObj.GetComponent<MeshRenderer>();
            if (r != null)
            {
                r.material = BadAppleManager.BadAppleMaterial;
                if (r.gameObject.GetComponent<BadAppleAnimator>() == null)
                    r.gameObject.AddComponent<BadAppleAnimator>();
            }
        }

        BadAppleManager.RebakeUVs(instance);
    }
}

