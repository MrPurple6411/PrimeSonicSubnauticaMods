namespace BetterBioReactor;

using System.Reflection;
using BepInEx;
using Common;
using HarmonyLib;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin: BaseUnityPlugin
{

    public void Awake()
    {
        QuickLogger.Info("Start patching. Version: " + QuickLogger.GetAssemblyVersion());

#if DEBUG
        QuickLogger.DebugLogsEnabled = true;
        QuickLogger.Debug("Debug logs enabled");
#endif
        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        QuickLogger.Info("Finished patching");
    }
}
