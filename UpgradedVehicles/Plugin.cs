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
    internal const string WorkBenchTab = "HullArmor";

    internal static SpeedBooster SpeedBooster { get; private set; }
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

            CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, WorkBenchTab, "Armor Modules", SpriteManager.Get(TechType.VehicleArmorPlating));

            //Handle SpeedBooster
            SpeedBooster = new SpeedBooster();

            //Handle HullArmorUpgrades
            HullArmorMk2 = new HullArmorMk2();
            HullArmorMk3 = new HullArmorMk3();
            HullArmorMk4 = new HullArmorMk4();

            //Register the upgrades
            SpeedBooster.CustomPrefab.Register();
            HullArmorMk2.CustomPrefab.Register();
            HullArmorMk3.CustomPrefab.Register();
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

        bool moreSeamothDepth = PluginInfos.ContainsKey("MoreSeamothDepth");
        if (moreSeamothDepth &&
            EnumHandler.TryGetValue("SeamothHullModule4", out TechType vehicleHullModule4) &&
            EnumHandler.TryGetValue("SeamothHullModule5", out TechType vehicleHullModule5))
        {
            QuickLogger.Info("Detected Seamoth Depth Modules Mk4 & Mk5");
            VehicleUpgrader.SeamothDepthModules.Add(vehicleHullModule4, 4);
            VehicleUpgrader.SeamothDepthModules.Add(vehicleHullModule5, 5);
            VehicleUpgrader.CommonUpgradeModules.Add(vehicleHullModule4);
            VehicleUpgrader.CommonUpgradeModules.Add(vehicleHullModule5);
            VehicleUpgrader.DepthUpgradeModules.Add(vehicleHullModule4);
            VehicleUpgrader.DepthUpgradeModules.Add(vehicleHullModule5);
        }
    }
}
