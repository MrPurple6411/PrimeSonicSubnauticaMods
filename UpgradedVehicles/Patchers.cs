namespace UpgradedVehicles;

using Common;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(DamageSystem), nameof(DamageSystem.CalculateDamage), new[] { typeof(float), typeof(DamageType), typeof(GameObject), typeof(GameObject) })]
internal class DamageSystem_CalculateDamage_Patch
{
    [HarmonyPostfix]
    internal static void Postfix(ref float __result, GameObject target)
    {
        Vehicle vehicle = target.GetComponent<Vehicle>();

        if (vehicle != null) // Target is vehicle
        {
            VehicleUpgrader vehicleUpgrader = vehicle.gameObject.GetComponent<VehicleUpgrader>();
            if(vehicleUpgrader != null)
                __result = vehicleUpgrader.GeneralDamageReduction * __result;
        }
    }
}

[HarmonyPatch(typeof(Vehicle))]
[HarmonyPatch(nameof(Vehicle.LazyInitialize))]
internal class Vehicle_LazyInitialize_Patcher
{

    private static bool Initialized = false;

    [HarmonyPrefix]
    internal static void Prefix(ref Vehicle __instance)
    {
        Initialized = __instance.isInitialized;
    }

    [HarmonyPostfix]
    internal static void Postfix(ref Vehicle __instance)
    {
        if(!Initialized)
        {
            QuickLogger.Debug(nameof(Vehicle_LazyInitialize_Patcher));
            __instance.gameObject.EnsureComponent<VehicleUpgrader>()?.Initialize(ref __instance);
        }
    }
}