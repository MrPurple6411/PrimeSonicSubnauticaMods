#if BELOWZERO
namespace UnSlowSeaTruck;

using System.Collections.Generic;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
[BepInProcess("SubnauticaZero.exe")]
public class Plugin: BaseUnityPlugin
{
    public static new SeaTruckConfig Config { get; } = new();
    internal static new ManualLogSource Logger { get; private set; }
    public Harmony Harmony { get; private set; }

    public void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo("Patching v" + MyPluginInfo.PLUGIN_VERSION);

        Config.Load();
        Logger.LogDebug($"Configured WeightOverride: {Config.WeightOverride}");
        Logger.LogDebug($"Configured SteeringMultiplier: {Config.SteeringMultiplier}");
        Logger.LogDebug($"Configured AccelerationMultiplier: {Config.AccelerationMultiplier}");
        Logger.LogDebug($"Configured HorsePowerModifier: {Config.HorsePowerModifier}");

        Harmony = Harmony.CreateAndPatchAll(typeof(Plugin), MyPluginInfo.PLUGIN_GUID);
    }

    [HarmonyPatch(typeof(SeaTruckSegment), nameof(SeaTruckSegment.GetAttachedWeight))]
    [HarmonyPostfix]
    internal static void SeaTruckSegment_GetAttachedWeight_Override_Postfix(ref float __result)
    {
        __result *= Config.WeightOverride; // Override attached module weight
    }

    [HarmonyPatch(typeof(SeaTruckMotor), nameof(SeaTruckMotor.Start))]
    [HarmonyPostfix]
    internal static void SeatruckMotor_Start_Change_Postfix(ref SeaTruckMotor __instance)
    {
        Logger.LogDebug($"Original steeringMultiplier: {__instance.steeringMultiplier}");
        Logger.LogDebug($"Original acceleration: {__instance.acceleration}");

        __instance.steeringMultiplier *= Config.SteeringMultiplier; // better steering
        __instance.acceleration *= Config.AccelerationMultiplier; // better acceleration

        Logger.LogDebug($"Modified steeringMultiplier: {__instance.steeringMultiplier} [Config:{Config.SteeringMultiplier}] (More is faster)");
        Logger.LogDebug($"Modified acceleration: {__instance.acceleration} [Config:{Config.AccelerationMultiplier}] (More is faster)");
    }

    [HarmonyPatch(typeof(SeaTruckMotor), nameof(SeaTruckMotor.GetWeight))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> SeatruckMotor_GetWeight_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var allCodes = new List<CodeInstruction>(instructions);
        CodeInstruction instructionToAlter = null;

        foreach (var item in allCodes)
        {
            if (item.opcode != OpCodes.Ldc_R4)
                continue;

            // Just in case they ever change the values or change the order of the operation
            if (instructionToAlter != null && (float)instructionToAlter.operand <= (float)item.operand)
                continue;

            instructionToAlter = item;
        }

        Logger.LogDebug($"Original HorsePowerModifier: {instructionToAlter.operand}");
        instructionToAlter.operand = Config.HorsePowerModifier;
        Logger.LogDebug($"Modified HorsePowerModifier: {instructionToAlter.operand} (Less is faster)");

        return allCodes;
    }
}
#endif