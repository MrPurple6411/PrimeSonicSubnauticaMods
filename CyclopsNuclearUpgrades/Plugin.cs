﻿namespace CyclopsNuclearUpgrades;

using BepInEx;
using Common;
using CyclopsNuclearUpgrades.Management;
using MoreCyclopsUpgrades.API;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.mrpurple6411.MoreCyclopsUpgrades", BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BaseUnityPlugin
{
    public void Patch()
    {
        QuickLogger.Info("Started patching v" + QuickLogger.GetAssemblyVersion());

        var nuclearModule = new CyclopsNuclearModule();
        var depletedModule = new DepletedNuclearModule(nuclearModule);

        nuclearModule.Patch();
        depletedModule.Patch();

        var nuclearFabricator = new NuclearFabricator(nuclearModule);
        nuclearFabricator.AddCraftNode(TechType.ReactorRod);
        nuclearFabricator.AddCraftNode(nuclearModule.TechType);
        nuclearFabricator.AddCraftNode("RReactorRodDUMMY"); // Optional - Refill nuclear reactor rod (old)
        nuclearFabricator.AddCraftNode("ReplenishReactorRod"); // Optional - Refill nuclear reactor rod (new)
        nuclearFabricator.AddCraftNode("CyNukeUpgrade1"); // Optional - Cyclops Nuclear Reactor Enhancer Mk1
        nuclearFabricator.AddCraftNode("CyNukeUpgrade2"); // Optional - Cyclops Nuclear Reactor Enhancer Mk2
        nuclearFabricator.Patch();

        MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
        {
            return new NuclearUpgradeHandler(nuclearModule.TechType, cyclops);
        });

        MCUServices.Register.CyclopsCharger<NuclearChargeHandler>((SubRoot cyclops) =>
        {
            return new NuclearChargeHandler(cyclops, nuclearModule.TechType);
        });

        MCUServices.Register.PdaIconOverlay(nuclearModule.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
        {
            return new NuclearIconOverlay(icon, upgradeModule);
        });

        QuickLogger.Info("Finished patching");
    }
}
