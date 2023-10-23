namespace CyclopsEngineUpgrades.Craftables;

using System.IO;
using System.Reflection;
using MoreCyclopsUpgrades.API.Upgrades;
using Nautilus.Crafting;
using static CraftData;

internal class PowerUpgradeModuleMk3 : CyclopsUpgrade
{
    public static PowerUpgradeModuleMk2 PreviousTier { get; internal set; }

    public PowerUpgradeModuleMk3()
        : base("PowerUpgradeModuleMk3",
              "Cyclops Engine Efficiency Module MK3",
              "Maximum engine efficiency. Silent running, Sonar, and Shield greatly optimized.\n" +
              "Does not stack with other engine upgrades.")
    {
    }

    public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Workbench;
    public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
    public override string[] StepsToFabricatorTab { get; } = new[] { "CyclopsMenu" };

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            craftAmount = 1,
            Ingredients =
            {
                new Ingredient(PreviousTier.TechType, 1),
                new Ingredient(TechType.PrecursorIonCrystal, 1),
                new Ingredient(TechType.Nickel, 1),
            }
        };
    }
}
