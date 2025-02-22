namespace CyclopsEnhancedSonar;

using System;
using BepInEx;
using Common;
using HarmonyLib;
using MoreCyclopsUpgrades.API;
using Nautilus.Handlers;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
[BepInDependency("com.mrpurple6411.MoreCyclopsUpgrades", BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

        try
        {
            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.Patch( // Create a postfix patch on the SubControl Start method to add the CySonarComponent
                original: AccessTools.Method(typeof(SubControl), nameof(SubControl.Start)), 
                postfix: new HarmonyMethod(typeof(Plugin), nameof(Plugin.SubControlStartPostfix)));

            // Register a custom upgrade handler for the CyclopsSonarModule
            MCUServices.Register.CyclopsUpgradeHandler((SubRoot s) => new SonarUpgradeHandler(s));

            // Register a PDA Icon Overlay for the CyclopsSonarModule
            MCUServices.Register.PdaIconOverlay(TechType.CyclopsSonarModule, 
                                                (uGUI_ItemIcon i, InventoryItem u) => new SonarPdaDisplay(i, u));

            // Add a language line for the text in the SonarPdaDisplay to allow it to be easily overridden
            LanguageHandler.SetLanguageLine(SonarPdaDisplay.SpeedUpKey, SonarPdaDisplay.SpeedUpText);

            QuickLogger.Info($"Finished patching.");
        }
        catch (Exception ex)
        {
            QuickLogger.Error(ex);
        }
    }

    internal static void SubControlStartPostfix(SubControl __instance)
    {
        if (__instance.gameObject.name.StartsWith("Cyclops-MainPrefab"))
            __instance.gameObject.AddComponent<CySonarComponent>();
    }
}
