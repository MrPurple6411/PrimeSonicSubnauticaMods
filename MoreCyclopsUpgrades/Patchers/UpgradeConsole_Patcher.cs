namespace MoreCyclopsUpgrades.Patchers;

using System.Collections.Generic;
using System.IO;
using Common;
using HarmonyLib;
using MoreCyclopsUpgrades.API.Buildables;
using Newtonsoft.Json;

[HarmonyPatch(typeof(UpgradeConsole))]
internal static class UpgradeConsole_OnProtoDeserializeObjectTree_Patcher
{

    [HarmonyPatch(nameof(UpgradeConsole.OnProtoDeserializeObjectTree))]
    [HarmonyPrefix]
    public static void Prefix(UpgradeConsole __instance)
    {
        if (__instance is AuxiliaryUpgradeConsole auxiliary)
        {
            var saveFolder = Path.Combine(SaveLoadManager.GetTemporarySavePath(), "MoreCyclopsUpgrades");
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            var uniqueIdentifier = auxiliary.GetComponentInParent<UniqueIdentifier>(true);

            if (uniqueIdentifier == null)
            {
                QuickLogger.Error($"Failed to load MoreCyclopsUpgrades Save Data as no identifier could be found!!!!!", true);
                return;
            }

            var savePath = Path.Combine(saveFolder, $"MCUSaveData_{uniqueIdentifier.Id}.json");
            try
            {
                if (File.Exists(savePath))
                    auxiliary.serializedModuleSlots = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(savePath));
                else
                    QuickLogger.Debug($"No MoreCyclopsUpgrades Save Data found at {savePath}", true);

            }
            catch (System.Exception e)
            {
                QuickLogger.Error($"Failed to load MoreCyclopsUpgrades Save Data at {savePath}!!!!!", true);
                QuickLogger.Error(e.Message);
            }
        }
    }

    [HarmonyPatch(nameof(UpgradeConsole.OnProtoSerialize))]
    [HarmonyPostfix]
    public static void Postfix(UpgradeConsole __instance)
    {
        if (__instance is AuxiliaryUpgradeConsole auxiliary)
        {
            var saveFolder = Path.Combine(SaveLoadManager.GetTemporarySavePath(), "MoreCyclopsUpgrades");
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            var uniqueIdentifier = auxiliary.GetComponentInParent<UniqueIdentifier>();
            var savePath = Path.Combine(saveFolder, $"MCUSaveData_{uniqueIdentifier.Id}.json");
            try
            {
                File.WriteAllText(savePath, JsonConvert.SerializeObject(auxiliary.serializedModuleSlots, Formatting.Indented));

                QuickLogger.Debug($"Saved MoreCyclopsUpgrades Save Data to {savePath}", true);
            }
            catch (System.Exception e)
            {
                QuickLogger.Error($"Failed to save MoreCyclopsUpgrades Save Data at {savePath}!!!!!", true);
                QuickLogger.Error(e.Message);
            }
        }
    }
}