namespace CyclopsAutoZapper;

using System;
using System.IO;
using System.Reflection;
using CyclopsAutoZapper.Managers;
using MoreCyclopsUpgrades.API;
using MoreCyclopsUpgrades.API.Upgrades;
using Nautilus.Crafting;
using Nautilus.Handlers;
using static CraftData;

internal class CyclopsAutoDefenseMk2 : CyclopsUpgrade
{
    private static TechType CyclopsZapperModule;

    public CyclopsAutoDefenseMk2()
        : base("CyclopsZapperModuleMk2",
               "Cyclops Auto Defense System Mk2",
               "Self contained, automated, anti-predator electrical defense system for the Cyclops.")
    {
        OnFinishedPatching += () =>
        {
            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            { return new UpgradeHandler(this.TechType, cyclops) { MaxCount = 1 }; });

            MCUServices.Register.PdaIconOverlay(this.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
            { return new AutoDefenseMk2IconOverlay(icon, upgradeModule); });

            MCUServices.Register.AuxCyclopsManager<AutoDefenserMk2>((SubRoot cyclops) =>
            { return new AutoDefenserMk2(this.TechType, cyclops); });
        };
    }

    public override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
    public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
    public override TechType RequiredForUnlock => TechType.SeamothElectricalDefense;
    public override string[] StepsToFabricatorTab { get; } = new[] { "CyclopsMenu" };

    internal static void CreateAndRegister(TechType cyclopsZapperModule)
    {
        CyclopsZapperModule = cyclopsZapperModule;
        new CyclopsAutoDefenseMk2().Patch();
    }

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData
        {
            craftAmount = 1,
            Ingredients =
            {
                new Ingredient(CyclopsZapperModule, 1),
                new Ingredient(TechType.SeamothElectricalDefense, 1),
                new Ingredient(TechType.PowerCell, 1),
                new Ingredient(TechType.Magnetite, 1)
            }
        };
    }
}
