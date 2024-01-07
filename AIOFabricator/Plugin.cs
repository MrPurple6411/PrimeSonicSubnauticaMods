namespace AIOFabricator;

using System.Collections;
using BepInEx;
using Common;
using UnityEngine;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.mrpurple6411.CustomCraft3", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin: BaseUnityPlugin
{
    public IEnumerator Start()
    {
        while (Language.main is null
#if BELOWZERO
            || !SpriteManager.hasInitialized
#endif
            )
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(5f);

        QuickLogger.Info($"Started patching v: {MyPluginInfo.PLUGIN_VERSION}");         

        AiOFab.CreateAndRegister();

        QuickLogger.Info("Finished patching");
    }        
}
