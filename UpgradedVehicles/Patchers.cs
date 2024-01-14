namespace UpgradedVehicles;

using System.Collections;
using System.Collections.Generic;
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
        Vehicle vehicle = target.GetComponent<Vehicle>();

        if (vehicle != null) // Target is vehicle
        {
            VehicleUpgradeHandler vehicleUpgrader = vehicle.gameObject.GetComponent<VehicleUpgradeHandler>();
            if (vehicleUpgrader != null)
            {
                // debug log before and after
                QuickLogger.Debug($"DamageSystem.CalculateDamage: {__result} => {vehicleUpgrader.GeneralDamageReduction * __result}", true);
                __result = vehicleUpgrader.GeneralDamageReduction * __result;
            }
        }
    }

    [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.LazyInitialize)), HarmonyPostfix]
    internal static void LazyInitializePostfix(ref Vehicle __instance)
    {
        if (__instance.gameObject.GetComponent<VehicleUpgradeHandler>() == null)
        {
            QuickLogger.Debug(nameof(Vehicle.LazyInitialize));
            __instance.gameObject.AddComponent<VehicleUpgradeHandler>()?.Initialize(ref __instance);
        }
    }
}