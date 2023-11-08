namespace CyclopsAutoZapper;

using System.Reflection;
using BepInEx;
using Common;
using HarmonyLib;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
[BepInDependency("com.mrpurple6411.MoreCyclopsUpgrades", BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BaseUnityPlugin
{
    public void Awake()
    {
        QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

        var defenseSystem = new CyclopsAutoDefense();
        defenseSystem.Patch();

        var antiParasites = new CyclopsParasiteRemover();
        antiParasites.Patch();

        var defenseSystemMk2 = new CyclopsAutoDefenseMk2(defenseSystem);
        defenseSystemMk2.Patch();

        DisplayTexts.Main.Patch();

        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        QuickLogger.Info("Finished Patching");
    }
}
