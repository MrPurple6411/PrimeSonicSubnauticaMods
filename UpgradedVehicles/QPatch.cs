﻿namespace UpgradedVehicles
{
    using System;
    using System.Reflection;
    using Common;
    using Harmony;
    using SMLHelper.V2.Handlers;
    using UpgradedVehicles.SaveData;

    public class QPatch
    {
        internal const string WorkBenchTab = "HullArmor";

        public static void Patch()
        {
            try
            {
                QuickLogger.Info("Started patching - " + QuickLogger.GetAssemblyVersion());

                //Handle CrossMod Updates
                if (TechTypeHandler.TryGetModdedTechType("SeamothHullModule4", out TechType vehicleHullModule4) &&
                    TechTypeHandler.TryGetModdedTechType("SeamothHullModule5", out TechType vehicleHullModule5))
                {
                    VehicleUpgrader.SeamothDepthModules.Add(vehicleHullModule4, 4);
                    VehicleUpgrader.SeamothDepthModules.Add(vehicleHullModule5, 5);
                    VehicleUpgrader.CommonUpgradeModules.Add(vehicleHullModule4);
                    VehicleUpgrader.CommonUpgradeModules.Add(vehicleHullModule5);
                }

                CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, WorkBenchTab, "Armor Modules", SpriteManager.Get(TechType.VehicleArmorPlating));

                //Handle SpeedBooster
                var speedModule = new SpeedBooster();
                speedModule.Patch();

                //Handle HullArmorUpgrades
                var hullArmorMk2Module = new HullArmorMk2();
                hullArmorMk2Module.Patch();

                var hullArmorMk3Module = new HullArmorMk3(hullArmorMk2Module.TechType);
                hullArmorMk3Module.Patch();

                var hullArmorMk4Module = new HullArmorMk4(hullArmorMk3Module.TechType);
                hullArmorMk4Module.Patch();

                //Handle Config Options
                var configOptions = new UpgradeOptions();
                configOptions.Initialize();

                VehicleUpgrader.SetBonusSpeedMultipliers(configOptions.SeamothBonusSpeedMultiplier, configOptions.ExosuitBonusSpeedMultiplier);

                var harmony = HarmonyInstance.Create("com.upgradedvehicles.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                QuickLogger.Info("Finished patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
