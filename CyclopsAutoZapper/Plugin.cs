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
    private void Awake()
    {
        QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

        CyclopsAutoDefense.CreateAndRegister();

        var antiParasites = new CyclopsParasiteRemover();
        antiParasites.Patch();

        DisplayTexts.Main.Patch();

        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        QuickLogger.Info("Finished Patching");
    }
}
