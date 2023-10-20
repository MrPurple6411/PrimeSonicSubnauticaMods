﻿namespace CyclopsAutoZapper;

using System.Reflection;
using BepInEx;
using Common;
using HarmonyLib;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.mrpurple6411.MoreCyclopsUpgrades", BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BaseUnityPlugin
{
    public void Awake()
    {
        QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());
        QuickLogger.DebugLogsEnabled = false;

        var defenseSystem = new CyclopsAutoDefense();
        defenseSystem.Patch();

        var antiParasites = new CyclopsParasiteRemover();
        antiParasites.Patch();

        var defenseSystemMk2 = new CyclopsAutoDefenseMk2(defenseSystem);
        defenseSystemMk2.Patch();

        DisplayTexts.Main.Patch();

        var harmony = new Harmony("com.cyclopsautozapper.psmod");
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        QuickLogger.Info("Finished Patching");
    }
}
