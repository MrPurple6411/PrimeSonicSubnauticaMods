namespace BetterBioReactor.Patchers;

using HarmonyLib;

[HarmonyPatch(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnOpenPDA))]
public class UGUI_InventoryTab_OnOpenPDA_Patcher
{
    [HarmonyPostfix]
    public static void Postfix(uGUI_InventoryTab __instance)
    {
        // This event happens whenever the player opens their PDA.
        // We will make a series of checks to see if what they have opened is the Base BioReactor item container.

        if (__instance == null || !BioReactorController.PdaIsOpen)
            return;

        ItemsContainer currentContainer = __instance.storage.container;
        BioReactorController reactor = BioReactorController.OpenInPda;
        if (reactor.BioReactor.container != currentContainer)
            return;

        reactor.ConnectToInventory(__instance.storage.items);
    }
}
