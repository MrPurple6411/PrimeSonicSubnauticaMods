namespace UpgradedVehicles.Handlers;

using System;
using System.Collections.Generic;
using Common;
using Nautilus.Options;
using UnityEngine;
using UpgradedVehicles.SaveData;

/// <summary>
/// MonoBehaviour that is attached to all vehicles using a patch to the Vehicle
/// </summary>
sealed class VehicleUpgradeHandler : VehicleAccelerationModifier
{
    internal static void OnBonusSpeedStyleChanged(object sender, ChoiceChangedEventArgs<BonusSpeedStyles> args)
    {
        if (!TechTypeExtensions.FromString(args.Id.Replace(ConfigSaveData.BonusSpeedSuffix, string.Empty).Trim(), out TechType techType, true))
        {
            QuickLogger.Error($"OnBonusSpeedStyleChanged - Unable to parse TechType from {args.Id}");
            return;
        }

        BonusSpeedStyles oldValue = Plugin.SaveData.GetBonusSpeedStyle(techType) ?? BonusSpeedStyles.Normal;
        BonusSpeedStyles newValue = args.Value;

        if (oldValue != newValue)
        {
            QuickLogger.Debug($"OnBonusSpeedStyleChanged - {techType} - {oldValue} => {newValue}");
            Plugin.SaveData.SetBonusSpeedStyle(techType, newValue);
        }

        if (vehicleUpgradeHandlers.TryGetValue(techType, out var handlers))
        {
            foreach (var handler in handlers)
            {
                handler.BonusSpeedStyle = newValue;
                handler.ProcessChanges(true, true, true);
            }
        }

        Plugin.SaveData.Save();
    }

    // All upgrade modules that we handle
    private static readonly HashSet<TechType> CommonUpgradeModules = new HashSet<TechType>()
    {
        TechType.ExoHullModule2,
        TechType.ExoHullModule1,
        TechType.VehicleHullModule3,
        TechType.VehicleHullModule2,
        TechType.VehicleHullModule1,
        TechType.VehiclePowerUpgradeModule,
        TechType.VehicleArmorPlating,
        // SpeedBooster added during patching
        // HullArmorUpgrades added during patching
        // Depth modules optionally added during patching
    };

    // All upgrade modules that affect armor
    private static readonly IDictionary<TechType, int> ArmorPlatingModules = new Dictionary<TechType, int>()
    {
        { TechType.VehicleArmorPlating, 1 }
        // HullArmorUpgrades added during patching
    };

    // All upgrade modules that affect speed
    private static readonly IDictionary<TechType, int> SpeedBoostingModules = new Dictionary<TechType, int>()
    {
        // SpeedUpgrades added during patching
    };

    // All upgrade modules that affect power
    private static readonly IDictionary<TechType, int> PowerRatingModules = new Dictionary<TechType, int>()
    {
        { TechType.VehiclePowerUpgradeModule, 1 }
        // EngineEfficiencyUpgrades added during patching
    };

    // All upgrade modules that affect depth
    private static readonly IDictionary<TechType, int> DepthUpgradeModules = new Dictionary<TechType, int>()
    {
        { TechType.ExoHullModule2, 2 },
        { TechType.ExoHullModule1, 1 },
        { TechType.VehicleHullModule3, 3 },
        { TechType.VehicleHullModule2, 2 },
        { TechType.VehicleHullModule1, 1 },
        // Depth Modules Mk4 and Mk5 optionaly added during patching
    };

    // Speed multiplier bonus per speed module.
    private static readonly IDictionary<TechType, float[]> VehicleBonusSpeedMultipliers = new Dictionary<TechType, float[]>()
    {
        { TechType.Exosuit, new float[]{ 0f, 0.1f, 0.25f, 0.45f } },
        { TechType.Seamoth, new float[]{ 0f, 0.05f, 0.15f, 0.3f } },
    };

    private static readonly IDictionary<TechType, List<VehicleUpgradeHandler>> vehicleUpgradeHandlers = new Dictionary<TechType, List<VehicleUpgradeHandler>>();

    private Vehicle ParentVehicle = null;
    private Equipment UpgradeModules => ParentVehicle.modules;

    private DealDamageOnImpact _dmgOnImpact;
    private DealDamageOnImpact DmgOnImpact => _dmgOnImpact ?? (_dmgOnImpact = ParentVehicle.GetComponent<DealDamageOnImpact>());

    internal float GeneralDamageReduction { get; private set; } = 1f;

    private bool Initialized;

    public int DepthIndex { get; private set; } = -1;
    public float EfficiencyPenalty { get; private set; } = 1f;
    public float EfficientyBonus { get; private set; } = 1f;
    public float GeneralArmorFraction { get; private set; } = 1f;
    public float ImpactArmorFraction { get; private set; } = 1f;
    public float PowerRating { get; private set; } = 1f;
    public LiveMixin LifeMix => ParentVehicle.liveMixin;

    public BonusSpeedStyles BonusSpeedStyle { get; private set; }

    /// <summary>
    /// Calculates the speed multiplier based on the number of speed modules and current depth index.
    /// </summary>
    private float GetSpeedMultiplierBonus(float speedBoosterCount)
    {
        float speedBoosterRatio = 0.15f;

        if (VehicleBonusSpeedMultipliers.TryGetValue(CraftData.GetTechType(ParentVehicle.gameObject), out float[] speedBoosterMultipliers))
        {
            int multiplierIndex = (int)this.BonusSpeedStyle;
            speedBoosterRatio = speedBoosterMultipliers.Length > multiplierIndex ? speedBoosterMultipliers[multiplierIndex] : speedBoosterMultipliers[0];
        }
        else // Most likely a new modded vehicle. Use the default speed multiplier.
        {
            QuickLogger.Warning($"GetSpeedMultiplierBonus - Unable to find speed multiplier for {ParentVehicle.gameObject.name} - Using default speed multiplier of 0.35f");
        }

        return 1f + // Base 100%
               ((speedBoosterRatio+0.2f) * speedBoosterCount) + // Bonus from Speed Boosters
               this.DepthIndex * speedBoosterRatio; // Bonus from Depth Modules
    }

    /// <summary>
    /// Calculates the engine efficiency penalty based off the number of speed modules and the current depth index.
    /// </summary>
    private float GetEfficiencyPentalty(float speedBoosterCount)
    {
        //  Speed Modules
        //    0    100%
        //    1    210%
        //    2    320%
        //    3    430%
        //    4    540%
        //    5    650%
        //    6    760%
        //    7    870%
        //    8    980%
        //    9    1090%

        return this.EfficiencyPenalty = 1f + 1.10f * speedBoosterCount;
    }

    /// <summary>
    /// Calculates the engine efficiency bonus based off the number of power modules and the current depth index.
    /// </summary>
    private float GetEfficiencyBonus(float powerModuleCount)
    {
        //                Depth Index
        //  Power Modules  0       1        2      3       4*      5*
        //        0       100%    115%    130%    145%    160%    175%
        //        1       200%    215%    230%    245%    260%    275%
        //        2       300%    315%    330%    345%    360%    375%
        //        3       400%    415%    430%    445%    460%    475%
        //        4       500%    515%    530%    545%    560%    575%
        //        5       600%    615%    630%    645%    660%    675%
        //        6       700%    715%    730%    745%    760%    775%
        //        7       800%    815%    830%    845%    860%    875%
        //        8       900%    915%    930%    945%    960%    975%
        //        9       1000%   1015%   1030%   1045%   1060%   1075%

        return this.EfficientyBonus = 1f + powerModuleCount + this.DepthIndex * 0.15f;
    }

    /// <summary>
    /// Calculates the general damage reduction fraction based off the number of hull armor modules and the current depth index.
    /// </summary>
    private float GetGeneralArmorFraction(float armorModuleCount)
    {
        float reduction = 0.20f * armorModuleCount;
        float bonus = 0.1f * this.DepthIndex;

        float damageReduction = Mathf.Max(1f - reduction - bonus, 0.01f);

        return this.GeneralArmorFraction = damageReduction;
    }

    /// <summary>
    /// Calculates the impact damage reduction fraction based off the number of hull armor modules and the current depth index.
    /// </summary>
    private float GetImpactArmorFraction(float armorModuleCount)
    {
        return this.ImpactArmorFraction = Mathf.Pow(0.5f, armorModuleCount + this.DepthIndex * 0.2f);
    }

    internal void Initialize<TVehicle>(ref TVehicle vehicle) where TVehicle : Vehicle
    {
        QuickLogger.Debug("Initializing vehicle", true);
        ParentVehicle = vehicle;

        if (this.UpgradeModules == null)
        {
            QuickLogger.Warning("Initialize Vehicle - UpgradeModules missing", true);
            DestroyImmediate(this);
            return;
        }

        this.UpgradeModules.onEquip += OnEquipmentChange;
        this.UpgradeModules.onUnequip += OnEquipmentChange;

        if (this.DmgOnImpact == null)
        {
            QuickLogger.Warning("Initialize Vehicle - DealDamageOnImpact missing", true);
            DestroyImmediate(this);
            return;
        }

        if (this.LifeMix == null)
        {
            QuickLogger.Warning("Initialize Vehicle - LiveMixin missing", true);
            DestroyImmediate(this);
            return;
        }

        TechType techType = CraftData.GetTechType(vehicle.gameObject);

        Plugin.SaveData.InitializeSaveFile();

        if (Plugin.SaveData.GetBonusSpeedStyle(techType) is BonusSpeedStyles bonusSpeedStyle)
        {
            QuickLogger.Debug($"Initialize Vehicle - {techType} - {bonusSpeedStyle}", true);
            this.BonusSpeedStyle = bonusSpeedStyle;
        }
        else
        {
            QuickLogger.Debug($"Initialize Vehicle - {techType} - Default", true);
            Plugin.SaveData.SetBonusSpeedStyle(techType, BonusSpeedStyles.Normal);
            this.BonusSpeedStyle = BonusSpeedStyles.Normal;
        }
        
        Plugin.SaveData.Save();
        
        ProcessChanges(true, true, true);

        Initialized = true;
    }

    private void OnDestroy()
    {
        if (this.UpgradeModules != null)
        {
            this.UpgradeModules.onEquip -= OnEquipmentChange;
            this.UpgradeModules.onUnequip -= OnEquipmentChange;
        }
    }

    private void OnEquipmentChange(string slot, InventoryItem item)
    {
        UpgradeVehicle(item._techType);
    }

    /// <summary>
    /// Calculated the number of armor plating modules installed based on their rating.
    /// Also gives a bonus based on the current depth index.
    /// </summary>
    private int CalculateArmorPlatingAmount()
    {
        int armorModuleCount = 0;
        foreach (var kvp in ArmorPlatingModules)
        {
            int moduleCount = this.UpgradeModules.GetCount(kvp.Key);
            armorModuleCount += moduleCount * kvp.Value;
        }

        return armorModuleCount;
    }

    /// <summary>
    /// Calculates the current depth index based on the highest depth module installed.
    /// </summary>
    private int CalculateDepthModuleIndex()
    {
        if (this.UpgradeModules == null)
            return -1; // Missing UpgradeModules, cannot calculate

        int maxDepthIndex = 0;
        foreach (var kvp in DepthUpgradeModules)
        {
            int moduleCount = this.UpgradeModules.GetCount(kvp.Key);
            if (moduleCount > 0)
                maxDepthIndex = Math.Max(maxDepthIndex, kvp.Value);
        }

        return 0;
    }

    /// <summary>
    /// Checks if the module that has been added or removed is a module we handle here.
    /// </summary>
    /// <param name="upgradeModule">The module that has been added or removed</param>
    internal void UpgradeVehicle(TechType upgradeModule)
    {
        if (!CommonUpgradeModules.Contains(upgradeModule))
            // Not an upgrade module we handle here
            return;

        bool updateAll = DepthUpgradeModules.ContainsKey(upgradeModule);
        bool updateArmor = updateAll || ArmorPlatingModules.ContainsKey(upgradeModule);
        bool updateSpeed = updateAll || SpeedBoostingModules.ContainsKey(upgradeModule);
        bool updateEfficiency = updateAll || updateSpeed || PowerRatingModules.ContainsKey(upgradeModule);
        QuickLogger.Debug($"updateAll:{updateAll} updateArmor:{updateArmor} updateSpeed:{updateSpeed} updateEfficiency:{updateEfficiency}");
        ProcessChanges(updateArmor, updateSpeed, updateEfficiency);
    }

    /// <summary>
    /// Processes the changes to the vehicle based on the upgrade modules that have been added or removed.
    /// </summary>
    /// <param name="updateArmor"> if true, the armor rating will be updated</param>
    /// <param name="updateSpeed"> if true, the speed rating will be updated</param>
    /// <param name="updateEfficiency"> if true, the efficiency rating will be updated</param>
    private void ProcessChanges(bool updateArmor, bool updateSpeed, bool updateEfficiency)
    {
        int newIndex = CalculateDepthModuleIndex();

        if (newIndex < 0)
        {
            QuickLogger.Error("UpgradeVehicle - UpgradeModules missing - Unable to upgrade", true);
            return;
        }

        this.DepthIndex = newIndex;

        if (updateArmor) // Armor
        {
            int armorModuleCount = CalculateArmorPlatingAmount();

            UpdateArmorRating(armorModuleCount);
        }

        if (updateEfficiency) // Efficiency
        {
            int speedBoostMultiplier = 0;
            int speedBoosterModuleCount = 0;
            foreach (var kvp in SpeedBoostingModules)
            {
                int moduleCount = this.UpgradeModules.GetCount(kvp.Key);
                speedBoostMultiplier += moduleCount * kvp.Value;
                speedBoosterModuleCount += moduleCount;

            }

            int powerModuleCount = 0;
            foreach (var kvp in PowerRatingModules)
            {
                int moduleCount = this.UpgradeModules.GetCount(kvp.Key);
                powerModuleCount += moduleCount * kvp.Value;
            }

            UpdatePowerRating(powerModuleCount, speedBoosterModuleCount);

            if (updateSpeed) // Speed
                UpdateSpeedRating(speedBoostMultiplier);
        }
    }

    /// <summary>
    /// Handles the speed rating based on the number of speed modules installed.
    /// </summary>
    /// <param name="speedBoosterCount"> The number of speed modules installed</param>
    private void UpdateSpeedRating(float speedBoosterCount)
    {
        QuickLogger.Debug($"UpdateSpeedRating - speedBoosterCount:{speedBoosterCount}", true);
        accelerationMultiplier = GetSpeedMultiplierBonus(speedBoosterCount);

        if (!Initialized)
            return;

        QuickLogger.Info($"Speed Boost: ({accelerationMultiplier * 100f:00}%)", true);
    }

    /// <summary>
    /// Handles the power rating based on the number of power modules and speed modules installed.
    /// </summary>
    /// <param name="powerModuleCount"> The number of power modules installed</param>
    /// <param name="speedBoosterCount"> The number of speed modules installed</param>
    private void UpdatePowerRating(int powerModuleCount, int speedBoosterCount)
    {
        QuickLogger.Debug($"UpdatePowerRating - powerModuleCount:{powerModuleCount} speedBoosterCount:{speedBoosterCount}", true);
        this.PowerRating = GetEfficiencyBonus(powerModuleCount) / GetEfficiencyPentalty(speedBoosterCount);

        ParentVehicle.enginePowerRating = this.PowerRating;

        if (!Initialized)
            return;

        QuickLogger.Info(Language.main.GetFormat("PowerRatingNowFormat", this.PowerRating), true);
    }

    /// <summary>
    /// Handles the armor rating based on the number of armor modules installed.
    /// </summary>
    /// <param name="armorModuleCount"> The number of armor modules installed</param>
    private void UpdateArmorRating(int armorModuleCount)
    {
        QuickLogger.Debug($"UpdateArmorRating - armorModuleCount:{armorModuleCount}", true);
        this.GeneralDamageReduction = GetGeneralArmorFraction(armorModuleCount);

        this.DmgOnImpact.mirroredSelfDamageFraction = GetImpactArmorFraction(armorModuleCount);

        if (!Initialized)
            return;

        QuickLogger.Info($"Armor rating is now {(1f - this.DmgOnImpact.mirroredSelfDamageFraction) * 100f + (1f - this.GeneralDamageReduction) * 100f:00}", true);
    }

    public static HashSet<TechType> GetVehicleTypes()
    {
        return new HashSet<TechType>(VehicleBonusSpeedMultipliers.Keys);
    }

    public static void RegisterVehicleSpeedOptions(TechType techType, float[] bonusSpeedsPerModule)
    {
        VehicleBonusSpeedMultipliers[techType] = bonusSpeedsPerModule;
    }

    /// <summary>
    /// Registers a module that affects how deep a vehicle can go.
    /// </summary>
    /// <param name="techType"> The module's TechType.</param>
    /// <param name="depthIndex"> The depth index of the module.</param>
    public static void RegisterDepthModule(TechType techType, int depthIndex)
    {
        if (DepthUpgradeModules.ContainsKey(techType))
        {
            QuickLogger.Warning($"RegisterDepthModule - {techType} already registered with depth index {DepthUpgradeModules[techType]} - Overwriting with depth index {depthIndex}");
            DepthUpgradeModules[techType] = depthIndex;
        }
        else
        {
            QuickLogger.Debug($"RegisterDepthModule - {techType} registered with depth index {depthIndex}");
            DepthUpgradeModules.Add(techType, depthIndex);
        }
        CommonUpgradeModules.Add(techType);
    }

    /// <summary>
    /// Registers a module that affects the speed boost of a vehicle.
    /// </summary>
    /// <param name="techType"> The module's TechType.</param>
    /// <param name="tier"> The how many modules this module is equivalent to.</param>
    public static void RegisterSpeedModule(TechType techType, int tier)
    {
        if (SpeedBoostingModules.ContainsKey(techType))
        {
            QuickLogger.Warning($"RegisterSpeedModule - {techType} already registered with tier {SpeedBoostingModules[techType]} - Overwriting with tier {tier}");
            SpeedBoostingModules[techType] = tier;
        }
        else
        {
            QuickLogger.Debug($"RegisterSpeedModule - {techType} registered with tier {tier}");
            SpeedBoostingModules.Add(techType, tier);
        }
        CommonUpgradeModules.Add(techType);
    }

    /// <summary>
    /// Registers a module that affects the power efficiency of a vehicle.
    /// </summary>
    /// <param name="techType"> The module's TechType.</param>
    /// <param name="tier"> The how many modules this module is equivalent to.</param>
    public static void RegisterPowerEfficiencyModule(TechType techType, int tier)
    {
        if (PowerRatingModules.ContainsKey(techType))
        {
            QuickLogger.Warning($"RegisterPowerRatingModule - {techType} already registered with tier {PowerRatingModules[techType]} - Overwriting with tier {tier}");
            PowerRatingModules[techType] = tier;
        }
        else
        {
            QuickLogger.Debug($"RegisterPowerRatingModule - {techType} registered with tier {tier}");
            PowerRatingModules.Add(techType, tier);
        }
        CommonUpgradeModules.Add(techType);
    }

    /// <summary>
    /// Registers a module that affects the armor rating of a vehicle.
    /// </summary>
    /// <param name="techType"> The module's TechType.</param>
    /// <param name="tier"> The how many modules this module is equivalent to.</param>
    public static void RegisterArmorPlatingModule(TechType techType, int tier)
    {
        if (ArmorPlatingModules.ContainsKey(techType))
        {
            QuickLogger.Warning($"RegisterArmorPlatingModule - {techType} already registered with tier {ArmorPlatingModules[techType]} - Overwriting with tier {tier}");
            ArmorPlatingModules[techType] = tier;
        }
        else
        {
            QuickLogger.Debug($"RegisterArmorPlatingModule - {techType} registered with tier {tier}");
            ArmorPlatingModules.Add(techType, tier);
        }
        CommonUpgradeModules.Add(techType);
    }

}
