namespace UpgradedVehicles;

using Common;
using HarmonyLib;
using UnityEngine;
using UpgradedVehicles.Handlers;

[HarmonyPatch]
internal static class Patchers
{
    [HarmonyPatch(typeof(DamageSystem), nameof(DamageSystem.CalculateDamage), new[] { typeof(float), typeof(DamageType), typeof(GameObject), typeof(GameObject) }), HarmonyPostfix]
    internal static void CalculateDamagePostfix(ref float __result, GameObject target)
    {
        VehicleUpgradeHandler vehicleUpgrader = target.GetComponent<VehicleUpgradeHandler>();

        if (vehicleUpgrader != null) // Target is vehicle
        {
            // debug log before and after
            QuickLogger.Debug($"DamageSystem.CalculateDamage: {__result} => {vehicleUpgrader.GeneralDamageReduction * __result}", true);
            __result = vehicleUpgrader.GeneralDamageReduction * __result;
        }
    }

    [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.LazyInitialize)), HarmonyPostfix]
    internal static void LazyInitializePostfix(ref Vehicle __instance)
    {
        if (!__instance.gameObject.TryGetComponent(out VehicleUpgradeHandler upgradeHandler))
        {
            QuickLogger.Debug(nameof(Vehicle.LazyInitialize));
            QuickLogger.Debug($"Adding VehicleUpgradeHandler to {__instance.gameObject.name}");
            upgradeHandler = __instance.gameObject.AddComponent<VehicleUpgradeHandler>();
        }

        if (!upgradeHandler.Initialized)
        {
            QuickLogger.Debug($"Initializing VehicleUpgradeHandler on {__instance.gameObject.name}");
            upgradeHandler.Initialize(__instance.gameObject);
        }
    }

    [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.Start)), HarmonyPostfix]
    internal static void SubRootStartPostfix(ref SubRoot __instance)
    {
        if (__instance is BaseRoot)
        {
            return;
        }

        if (!__instance.gameObject.TryGetComponent(out VehicleUpgradeHandler upgradeHandler))
        {
            QuickLogger.Debug(nameof(SubRoot.Start), true);
            QuickLogger.Debug($"Adding VehicleUpgradeHandler to {__instance.gameObject.name}");
            upgradeHandler = __instance.gameObject.AddComponent<VehicleUpgradeHandler>();
        }

        if (!upgradeHandler.Initialized)
        {
            QuickLogger.Debug($"Initializing VehicleUpgradeHandler on {__instance.gameObject.name}");
            upgradeHandler.Initialize(__instance.gameObject);
        }
    }

#if BELOWZERO
    [HarmonyPatch(typeof(SeaTruckMotor), nameof(SeaTruckMotor.Start)), HarmonyPostfix]
    internal static void SeaTruckUpgradesStartPostfix(ref SeaTruckMotor __instance)
    {
        if (!__instance.gameObject.TryGetComponent(out VehicleUpgradeHandler upgradeHandler))
        {
            QuickLogger.Debug(nameof(SeaTruckMotor.Start));
            QuickLogger.Debug($"Adding VehicleUpgradeHandler to {__instance.gameObject.name}");
            upgradeHandler = __instance.gameObject.AddComponent<VehicleUpgradeHandler>();
        }

        if (!upgradeHandler.Initialized)
        {
            QuickLogger.Debug($"Initializing VehicleUpgradeHandler on {__instance.gameObject.name}");
            upgradeHandler.Initialize(__instance.gameObject);
        }
    }

    [HarmonyPatch(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.IsAllowedToAdd)), HarmonyPrefix]
    internal static bool SeaTruckUpgradesIsAllowedToAddPrefix(ref SeaTruckUpgrades __instance, ref Pickupable pickupable, ref bool verbose, ref bool __result)
    {
        if (pickupable == null)
        {
            return true;
        }

        TechType techType = pickupable.GetTechType();

        if (VehicleUpgradeHandler.CommonUpgradeModules.Contains(techType) && !VehicleUpgradeHandler.DepthUpgradeModules.Keys.Contains(techType))
        {
            __result = true;
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(Hoverbike), nameof(Hoverbike.LazyInitialize)), HarmonyPostfix]
    internal static void HoverbikeStartPostfix(ref Hoverbike __instance)
    {
        if (!__instance.gameObject.TryGetComponent(out VehicleUpgradeHandler upgradeHandler))
        {
            QuickLogger.Debug(nameof(Hoverbike.Start));
            QuickLogger.Debug($"Adding VehicleUpgradeHandler to {__instance.gameObject.name}");
            upgradeHandler = __instance.gameObject.AddComponent<VehicleUpgradeHandler>();
        }

        if (!upgradeHandler.Initialized)
        {
            QuickLogger.Debug($"Initializing VehicleUpgradeHandler on {__instance.gameObject.name}");
            upgradeHandler.Initialize(__instance.gameObject);
        }
    }
#endif

    [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnEquip))]
    [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnUnequip))]
    [HarmonyPatch(typeof(UpgradeConsole), nameof(UpgradeConsole.OnEquip))]
    [HarmonyPatch(typeof(UpgradeConsole), nameof(UpgradeConsole.OnUnequip))]
#if BELOWZERO
    [HarmonyPatch(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.OnEquip))]
    [HarmonyPatch(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.OnUnequip))]
    [HarmonyPatch(typeof(Hoverbike), nameof(Hoverbike.OnEquip))]
    [HarmonyPatch(typeof(Hoverbike), nameof(Hoverbike.OnUnequip))]
#endif
    [HarmonyPostfix]
    internal static void EquipChangePostfix(object __instance, ref InventoryItem item)
    {
        TechType techType = item?.item?.GetTechType() ?? TechType.None;
        if (techType == TechType.None)
            return;

        switch (__instance)
        {
            case Vehicle vehicle:
            {
                QuickLogger.Debug($"Vehicle EquipChange: {vehicle.name} {techType}");
                VehicleUpgradeHandler vehicleUpgrader = vehicle.GetComponent<VehicleUpgradeHandler>();
                vehicleUpgrader.EnsureEquipmentRegistered(vehicle.modules);
                vehicleUpgrader.UpgradeVehicle(techType);
                break;
            }
#if BELOWZERO
            case SeaTruckUpgrades upgrades:
            {
                QuickLogger.Debug($"SeaTruckUpgrades EquipChange: {upgrades.name} {item.item.GetTechType()}");
                VehicleUpgradeHandler vehicleUpgrader = upgrades.motor.GetComponent<VehicleUpgradeHandler>();
                if (vehicleUpgrader == null)
                {
                    vehicleUpgrader = upgrades.motor.gameObject.AddComponent<VehicleUpgradeHandler>();
                    vehicleUpgrader.Initialize(upgrades.motor.gameObject);
                }
                vehicleUpgrader.EnsureEquipmentRegistered(upgrades.modules);
                vehicleUpgrader.UpgradeVehicle(techType);
                break;
            }
            case Hoverbike hoverbike:
            {
                QuickLogger.Debug($"Hoverbike EquipChange: {hoverbike.name} {item.item.GetTechType()}");
                VehicleUpgradeHandler vehicleUpgrader = hoverbike.GetComponent<VehicleUpgradeHandler>();
                vehicleUpgrader.EnsureEquipmentRegistered(hoverbike.modules);
                vehicleUpgrader.UpgradeVehicle(techType);
                break;
            }
#endif
            case UpgradeConsole console:
            {
                QuickLogger.Debug($"UpgradeConsole EquipChange: {console.name} {techType}");
                
                SubRoot subRoot = console.GetComponentInParent<SubRoot>();
                if (subRoot == null)
                {
                    QuickLogger.Error($"UpgradeConsole EquipChange: {console.name} {techType} - SubRoot not found");
                    return;
                }

                VehicleUpgradeHandler vehicleUpgrader = subRoot.GetComponent<VehicleUpgradeHandler>();
                vehicleUpgrader.EnsureEquipmentRegistered(console.modules);
                vehicleUpgrader.UpgradeVehicle(techType);
                break;
            }
        }
    }

    [HarmonyPatch(typeof(Equipment), nameof(Equipment.IsCompatible)), HarmonyPostfix]
    internal static void IsCompatiblePostfix(EquipmentType itemType, EquipmentType slotType, ref bool __result)
    {
        if (__result)
            return;

        if (slotType == EquipmentType.CyclopsModule && itemType == EquipmentType.VehicleModule)
            __result = true;

#if BELOWZERO
        if (slotType == EquipmentType.SeaTruckModule && itemType == EquipmentType.VehicleModule)
            __result = true;

        if (slotType == EquipmentType.HoverbikeModule && itemType == EquipmentType.VehicleModule)
            __result = true;
#endif

    }
}