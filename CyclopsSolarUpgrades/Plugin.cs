namespace CyclopsSolarUpgrades;

using System;
using BepInEx;
using Common;
using CyclopsSolarUpgrades.Craftables;
using CyclopsSolarUpgrades.Management;
using MoreCyclopsUpgrades.API;
using MoreCyclopsUpgrades.API.PDA;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
[BepInDependency("com.mrpurple6411.MoreCyclopsUpgrades", BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BaseUnityPlugin
{
    public void Awake()
    {
        try
        {
            QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            var solar1 = new CyclopsSolarCharger();
            solar1.Patch();

            CyclopsSolarChargerMk2.PreviousTier = solar1;
            var solar2 = new CyclopsSolarChargerMk2();
            solar2.Patch();

            MCUServices.Register.CyclopsCharger<SolarCharger>((SubRoot cyclops) =>
            {
                return new SolarCharger(solar1.TechType, solar2.TechType, cyclops);
            });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                return new SolarUpgradeHandler(solar1.TechType, solar2.TechType, cyclops);
            });

            MCUServices.Register.PdaIconOverlay(solar1.TechType, CreateIconOverlay);

            MCUServices.Register.PdaIconOverlay(solar2.TechType, CreateIconOverlay);

            QuickLogger.Info($"Finished patching.");
        }
        catch (Exception ex)
        {
            QuickLogger.Error(ex);
        }
    }

    internal static IconOverlay CreateIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
    {
        return new SolarIconOverlay(icon, upgradeModule);
    }
}
