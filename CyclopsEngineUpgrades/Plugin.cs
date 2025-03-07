﻿namespace CyclopsEngineUpgrades;

using BepInEx;
using Common;
using CyclopsEngineUpgrades.Craftables;
using CyclopsEngineUpgrades.Handlers;
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

        var engineMk2Upgrade = new PowerUpgradeModuleMk2();
        engineMk2Upgrade.Patch();

        PowerUpgradeModuleMk3.PreviousTier = engineMk2Upgrade;
        var engineMk3Upgrade = new PowerUpgradeModuleMk3();
        engineMk3Upgrade.Patch();

        LanguageHandler.SetLanguageLine(EngineOverlay.BonusKey, "[Bonus Efficiency]");
        LanguageHandler.SetLanguageLine(EngineOverlay.TotalKey, "[Total Efficiency]");

        MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
        {
            return new EngineHandler(engineMk2Upgrade, engineMk3Upgrade, cyclops);
        });

        MCUServices.Register.PdaIconOverlay(TechType.PowerUpgradeModule, CreateEngineOverlay);
        MCUServices.Register.PdaIconOverlay(engineMk2Upgrade.TechType, CreateEngineOverlay);
        MCUServices.Register.PdaIconOverlay(engineMk3Upgrade.TechType, CreateEngineOverlay);

        QuickLogger.Info($"Finished patching.");
    }

    internal static EngineOverlay CreateEngineOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
    {
        return new EngineOverlay(icon, upgradeModule);
    }
}
