// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace SynesthesiaDev.DressmakerBadApple;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource Log;

    protected void Awake()
    {
        Log = Logger;

        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        Logger.LogInfo("Harmony patches applied successfully :3");

        BadAppleManager.Initialize();
    }
}
