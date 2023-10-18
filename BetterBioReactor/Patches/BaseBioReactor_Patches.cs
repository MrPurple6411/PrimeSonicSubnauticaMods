namespace BetterBioReactor.Patchers;

using HarmonyLib;

[HarmonyPatch(typeof(BaseBioReactor))]
internal static class BaseBioReactor_Patches
{
    [HarmonyPatch(nameof(BaseBioReactor.Start))]
    [HarmonyPostfix]
    internal static void Start(BaseBioReactor __instance)
    {
        BioReactorController.GetMiniReactor(ref __instance).Start();
    }

    [HarmonyPatch(nameof(BaseBioReactor.OnHover))]
    [HarmonyPrefix]
    internal static bool OnHover(BaseBioReactor __instance)
    {
        BioReactorController.GetMiniReactor(ref __instance).OnHover();

        return false; // Full override
    }

    [HarmonyPatch(nameof(BaseBioReactor.OnUse))]
    [HarmonyPrefix]
    internal static bool OnUse(BaseBioReactor __instance, ref BaseBioReactorGeometry model)
    {
        BioReactorController.GetMiniReactor(ref __instance).OnPdaOpen(model);

        return false; // Full override
    }

    [HarmonyPatch(nameof(BaseBioReactor.Update))]
    [HarmonyPostfix]
    internal static void Update(BaseBioReactor __instance)
    {
        BioReactorController.GetMiniReactor(ref __instance).UpdateDisplayText();
    }

    [HarmonyPatch(nameof(BaseBioReactor.ProducePower))]
    [HarmonyPrefix]
    internal static bool ProducePower(BaseBioReactor __instance, float requested, ref float __result)
    {
        __result = BioReactorController.GetMiniReactor(ref __instance).ProducePower(requested);

        return false; // Full override
    }

    [HarmonyPatch(nameof(BaseBioReactor.OnProtoSerializeObjectTree))]
    [HarmonyPrefix]
    internal static void OnProtoSerializeObjectTree(BaseBioReactor __instance)
    {
        BioReactorController.GetMiniReactor(ref __instance).OnProtoSerializeObjectTree();
    }
}