namespace CyclopsNuclearUpgrades;

using BepInEx;
using Common;
using CyclopsNuclearUpgrades.Management;
using MoreCyclopsUpgrades.API;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.mrpurple6411.MoreCyclopsUpgrades", BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BaseUnityPlugin
{
    internal CyclopsNuclearModule NuclearModule { get; private set; }

    public void Awake()
    {
        QuickLogger.Info("Started patching v" + QuickLogger.GetAssemblyVersion());

        NuclearModule = new CyclopsNuclearModule();
        NuclearModule.Patch();

        DepletedNuclearModule.CreateAndRegister();
        NuclearFabricator.CreateAndRegister();

        MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
        {
            return new NuclearUpgradeHandler(NuclearModule.TechType, cyclops);
        });

        MCUServices.Register.CyclopsCharger<NuclearChargeHandler>((SubRoot cyclops) =>
        {
            return new NuclearChargeHandler(cyclops, NuclearModule.TechType);
        });

        MCUServices.Register.PdaIconOverlay(NuclearModule.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
        {
            return new NuclearIconOverlay(icon, upgradeModule);
        });

        QuickLogger.Info("Finished patching");
    }

    public void Start()
    {
        NuclearFabricator.Fabricator?.Root
            .AddCraftNode(TechType.ReactorRod)
            .AddCraftNode(NuclearModule.Info.TechType)
            .AddCraftNode("RReactorRodDUMMY") // Optional - Refill nuclear reactor rod (old)
            .AddCraftNode("ReplenishReactorRod") // Optional - Refill nuclear reactor rod (new)
            .AddCraftNode("CyNukeUpgrade1") // Optional - Cyclops Nuclear Reactor Enhancer Mk1
            .AddCraftNode("CyNukeUpgrade2"); // Optional - Cyclops Nuclear Reactor Enhancer Mk2

        QuickLogger.Info("Added Nuclear Fabricator crafting nodes");
    }
}
