namespace UpgradedVehicles;

using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Common;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Handlers;
using UnityEngine;
using UpgradedVehicles.Handlers;
using UpgradedVehicles.Modules.Armor;
using UpgradedVehicles.Modules.Power;
using UpgradedVehicles.Modules.Speed;
using UpgradedVehicles.SaveData;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
[BepInDependency("SeaTruckDepthUpgrades", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("MoreSeamothDepth", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
    internal const string WorkBenchArmorTab = "HullArmor";
    internal const string WorkBenchSpeedTab = "SpeedModules";
    internal const string WorkBenchPowerTab = "PowerModules";

    internal static UpgradeOptions Options { get; } = new UpgradeOptions();
    internal static ConfigSaveData SaveData { get; } = new ConfigSaveData();

    internal static SpeedBooster SpeedBooster { get; private set; }
    internal static SpeedBoosterMK2 SpeedBoosterMK2 { get; private set; }
    internal static SpeedBoosterMK3 SpeedBoosterMK3 { get; private set; }
    internal static SpeedBoosterMK4 SpeedBoosterMK4 { get; private set; }
    internal static HullArmorMk2 HullArmorMk2 { get; private set; }
    internal static HullArmorMk3 HullArmorMk3 { get; private set; }
    internal static HullArmorMk4 HullArmorMk4 { get; private set; }
    internal static PowerEfficiencyMK2 PowerEfficiencyMK2 { get; private set; }
    internal static PowerEfficiencyMK3 PowerEfficiencyMK3 { get; private set; }
    internal static PowerEfficiencyMK4 PowerEfficiencyMK4 { get; private set; }

    public Plugin()
    {
        OptionsPanelHandler.RegisterModOptions(Options);
    }

    public void Awake()
    {
        try
        {
            QuickLogger.Info("Started patching - " + QuickLogger.GetAssemblyVersion());
            CrossModUpdates();

            //Handle SpeedBooster
            SpeedBooster = new SpeedBooster();
            CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, WorkBenchSpeedTab, "Speed Modules", SpriteManager.Get(SpeedBooster.Info.TechType));
            SpeedBooster.CustomPrefab.Register();

            SpeedBoosterMK2 = new SpeedBoosterMK2();
            SpeedBoosterMK2.CustomPrefab.Register();
            SpeedBoosterMK3 = new SpeedBoosterMK3();
            SpeedBoosterMK3.CustomPrefab.Register();
            SpeedBoosterMK4 = new SpeedBoosterMK4();
            SpeedBoosterMK4.CustomPrefab.Register();

            //Handle HullArmorUpgrades
            CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, WorkBenchArmorTab, "Armor Modules", SpriteManager.Get(TechType.VehicleArmorPlating));
#if BELOWZERO
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, TechType.VehicleArmorPlating, WorkBenchArmorTab);
            KnownTechHandler.SetAnalysisTechEntry(TechType.Workbench,new TechType[] { TechType.VehicleArmorPlating });
            LanguageHandler.SetTechTypeName(TechType.VehicleArmorPlating, "Hull Reinforcement Module");
            LanguageHandler.SetTechTypeTooltip(TechType.VehicleArmorPlating, "An upgrade containing nanites improving and maintaining the inner structure of the hull.");
#endif
            HullArmorMk2 = new HullArmorMk2();
            HullArmorMk2.CustomPrefab.Register();
            HullArmorMk3 = new HullArmorMk3();
            HullArmorMk3.CustomPrefab.Register();
            HullArmorMk4 = new HullArmorMk4();
            HullArmorMk4.CustomPrefab.Register();

            //Handle PowerUpgrades
            CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, WorkBenchPowerTab, "Power Modules", SpriteManager.Get(TechType.VehiclePowerUpgradeModule));
#if BELOWZERO
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Workbench, TechType.VehiclePowerUpgradeModule, WorkBenchPowerTab);
            KnownTechHandler.SetAnalysisTechEntry(TechType.Workbench, new TechType[] { TechType.VehiclePowerUpgradeModule });
            LanguageHandler.SetTechTypeName(TechType.VehiclePowerUpgradeModule, "Engine Efficiency Module");
            LanguageHandler.SetTechTypeTooltip(TechType.VehiclePowerUpgradeModule, "Increases the efficiency of the engine.");
#endif

            PowerEfficiencyMK2 = new PowerEfficiencyMK2();
            PowerEfficiencyMK2.CustomPrefab.Register();

            PowerEfficiencyMK3 = new PowerEfficiencyMK3();
            PowerEfficiencyMK3.CustomPrefab.Register();

            PowerEfficiencyMK4 = new PowerEfficiencyMK4();
            PowerEfficiencyMK4.CustomPrefab.Register();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
            QuickLogger.Info("Finished patching");
        }
        catch (Exception ex)
        {
            QuickLogger.Error(ex);
        }
    }

    public static void CrossModUpdates()
    {
        QuickLogger.Info("Checking if MoreSeamothDepth mod is present");

        if (TechTypeExtensions.FromString("SeamothHullModule4", out TechType vehicleHullModule4, true))
        {
            QuickLogger.Info("Detected Seamoth Depth Module Mk4");            
            VehicleUpgradeHandler.RegisterDepthModule(vehicleHullModule4, 4);
        }

        if (TechTypeExtensions.FromString("SeamothHullModule5", out TechType vehicleHullModule5, true))
        {
            QuickLogger.Info("Detected Seamoth Depth Module Mk5");
            VehicleUpgradeHandler.RegisterDepthModule(vehicleHullModule5, 5);
        }

        if (TechTypeExtensions.FromString("ModVehicleDepthModule1" , out TechType modVehicleDepthModule1, true))
        {
            QuickLogger.Info("Detected Vehicle Framework Depth Module Mk1");
            VehicleUpgradeHandler.RegisterDepthModule(modVehicleDepthModule1, 1);
        }

        if (TechTypeExtensions.FromString("ModVehicleDepthModule2", out TechType modVehicleDepthModule2, true))
        {
            QuickLogger.Info("Detected Vehicle Framework Depth Module Mk2");
            VehicleUpgradeHandler.RegisterDepthModule(modVehicleDepthModule2, 2);
        }

        if (TechTypeExtensions.FromString("ModVehicleDepthModule3", out TechType modVehicleDepthModule3, true))
        {
            QuickLogger.Info("Detected Vehicle Framework Depth Module Mk3");
            VehicleUpgradeHandler.RegisterDepthModule(modVehicleDepthModule3, 3);
        }
    }
}
