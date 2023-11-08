namespace CyclopsThermalUpgrades;

using System;
using BepInEx;
using Common;
using CyclopsThermalUpgrades.Craftables;
using CyclopsThermalUpgrades.Management;
using MoreCyclopsUpgrades.API;
using MoreCyclopsUpgrades.API.PDA;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
[BepInDependency("com.mrpurple6411.MoreCyclopsUpgrades", BepInDependency.DependencyFlags.HardDependency)]
public class Plugin: BaseUnityPlugin
{
    public void Awake()
    {
        try
        {
            QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            var thermalMk2 = new CyclopsThermalChargerMk2();
            thermalMk2.Patch();

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                return new ThermalUpgradeHandler(TechType.CyclopsThermalReactorModule, thermalMk2.TechType, cyclops);
            });

            MCUServices.Register.CyclopsCharger<ThermalCharger>((SubRoot cyclops) =>
            {
                return new ThermalCharger(thermalMk2.TechType, cyclops);
            });

            MCUServices.Register.PdaIconOverlay(TechType.CyclopsThermalReactorModule, CreateIconOverlay);
            MCUServices.Register.PdaIconOverlay(thermalMk2.TechType, CreateIconOverlay);

            QuickLogger.Info($"Finished patching.");
        }
        catch (Exception ex)
        {
            QuickLogger.Error(ex);
        }
    }

    internal static IconOverlay CreateIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
    {
        return new ThermalIconOverlay(icon, upgradeModule);
    }
}
