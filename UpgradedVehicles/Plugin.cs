namespace UpgradedVehicles;

using System;
using System.Reflection;
using BepInEx;
using Common;
using HarmonyLib;
using Nautilus.Handlers;
using UpgradedVehicles.SaveData;
using static BepInEx.Bootstrap.Chainloader;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("SeaTruckDepthUpgrades", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("MoreSeamothDepth", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
    internal const string WorkBenchArmorTab = "HullArmor";
    internal const string WorkBenchSpeedTab = "SpeedModules";

    internal static SpeedBooster SpeedBooster { get; private set; }
    internal static SpeedBoosterMK2 SpeedBoosterMK2 { get; private set; }
    internal static SpeedBoosterMK3 SpeedBoosterMK3 { get; private set; }
    internal static SpeedBoosterMK4 SpeedBoosterMK4 { get; private set; }
    internal static HullArmorMk2 HullArmorMk2 { get; private set; }
    internal static HullArmorMk3 HullArmorMk3 { get; private set; }
    internal static HullArmorMk4 HullArmorMk4 { get; private set; }

    public void Awake()
    {
        try
        {
            QuickLogger.Info("Started patching - " + QuickLogger.GetAssemblyVersion());

            //Handle Config Options
            var configOptions = new UpgradeOptions();
            configOptions.Initialize();

            QuickLogger.DebugLogsEnabled = configOptions.DebugLogsEnabled;
            QuickLogger.Debug("Debug logs enabled");

            CrossModUpdates();

            
            

            //Handle SpeedBooster
            SpeedBooster = new SpeedBooster();
            SpeedBooster.CustomPrefab.Register();
            CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, WorkBenchSpeedTab, "Speed Modules", SpriteManager.Get(SpeedBooster.Info.TechType));
            
            SpeedBoosterMK2 = new SpeedBoosterMK2();
            SpeedBoosterMK2.CustomPrefab.Register();
            SpeedBoosterMK3 = new SpeedBoosterMK3();
            SpeedBoosterMK3.CustomPrefab.Register();
            SpeedBoosterMK4 = new SpeedBoosterMK4();
            SpeedBoosterMK4.CustomPrefab.Register();

            CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, WorkBenchArmorTab, "Armor Modules", SpriteManager.Get(TechType.VehicleArmorPlating));
            //Handle HullArmorUpgrades
            HullArmorMk2 = new HullArmorMk2();
            HullArmorMk2.CustomPrefab.Register();
            HullArmorMk3 = new HullArmorMk3();
            HullArmorMk3.CustomPrefab.Register();
            HullArmorMk4 = new HullArmorMk4();
            HullArmorMk4.CustomPrefab.Register();

            VehicleUpgrader.SetBonusSpeedMultipliers(configOptions);

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

        if (EnumHandler.TryGetValue("SeamothHullModule4", out TechType vehicleHullModule4))
        {
            QuickLogger.Info("Detected Seamoth Depth Module Mk4");
            VehicleUpgrader.SeamothDepthModules.Add(vehicleHullModule4, 4);
            VehicleUpgrader.CommonUpgradeModules.Add(vehicleHullModule4);
            VehicleUpgrader.DepthUpgradeModules.Add(vehicleHullModule4);
        }

        if (EnumHandler.TryGetValue("SeamothHullModule5", out TechType vehicleHullModule5))
        {
            QuickLogger.Info("Detected Seamoth Depth Module Mk5");
            VehicleUpgrader.SeamothDepthModules.Add(vehicleHullModule5, 5);
            VehicleUpgrader.CommonUpgradeModules.Add(vehicleHullModule5);
            VehicleUpgrader.DepthUpgradeModules.Add(vehicleHullModule5);
        }
    }
}
