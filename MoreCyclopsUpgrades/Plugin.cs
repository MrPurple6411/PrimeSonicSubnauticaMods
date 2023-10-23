namespace MoreCyclopsUpgrades;

using System.IO;
using System.Reflection;
using Common;
using MoreCyclopsUpgrades.AuxConsole;
using MoreCyclopsUpgrades.Config;
using MoreCyclopsUpgrades.Managers;
using Nautilus.Utility;
using HarmonyLib;
using BepInEx;

/// <summary>
/// Entry point class for patching. For use by Bepinex only.
/// </summary>
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BaseUnityPlugin
{
    /// <summary>
    /// Setting up the mod config.
    /// </summary>
    Plugin()
    {
        ModConfig.LoadOnDemand();
        string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        CyclopsHUDManager.CyclopsThermometer = ImageUtils.LoadSpriteFromFile(executingLocation + "/Assets/CyclopsThermometer.png");
    }

    /// <summary>
    /// Main patching method.
    /// </summary>
    public void Awake()
    {
        QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

        // If enabled, patch the Auxiliary Upgrade Console as a new buildable.
        if (ModConfig.Main.AuxConsoleEnabled)
            AuxCyUpgradeConsole.CreateAndRegister();
        else
            QuickLogger.Info("Auxiliary Upgrade Console disabled by config settings");

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);

        QuickLogger.Info("Finished Patching");
    }
}
