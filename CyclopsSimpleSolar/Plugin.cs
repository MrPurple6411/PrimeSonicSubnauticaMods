namespace CyclopsSimpleSolar;

using BepInEx;
using Common;
using MoreCyclopsUpgrades.API;
using MoreCyclopsUpgrades.API.Upgrades;
using Nautilus.Handlers;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.mrpurple6411.MoreCyclopsUpgrades", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.mrpurple6411.CyclopsSolarUpgrades", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin: BaseUnityPlugin
{
    private static TechType solarChargerMk1 = TechType.None;
    private static TechType solarChargerMk2 = TechType.None;
    internal const string CrossModKey = "CySolCross";

    public void Awake()
    {
        if (EnumHandler.TryGetValue("CyclopsSolarCharger", out solarChargerMk1) &&
            EnumHandler.TryGetValue("CyclopsSolarChargerMk2", out solarChargerMk2))
        {
            QuickLogger.Info("CyclopsSolarUpgrades mod is present. Solar charging will not stack with this mod.");
            QuickLogger.Debug("TechTypes for other Cyclops solar chargers detected, Aborting loading.");

            return;
        }

        QuickLogger.Info("Started patching v" + QuickLogger.GetAssemblyVersion());

        var solarChargerItem = new CySolarModule();
        solarChargerItem.Patch();

        MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
        {
            return new UpgradeHandler(solarChargerItem.TechType, cyclops)
            {
                MaxCount = 1
            };
        });

        MCUServices.Register.CyclopsCharger<CySolarChargeManager>((SubRoot cyclops) =>
        {
            return new CySolarChargeManager(solarChargerItem, cyclops)
            {
                OtherCySolarModsPresent = solarChargerMk2 > TechType.None,
                CrossModSolarCharger1 = solarChargerMk1,
                CrossModSolarCharger2 = solarChargerMk2
            };
        });

        MCUServices.Register.PdaIconOverlay(solarChargerItem.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
        {
            return new SolarPdaOverlay(icon, upgradeModule);
        });

        QuickLogger.Info("Finished patching");
    }
}
