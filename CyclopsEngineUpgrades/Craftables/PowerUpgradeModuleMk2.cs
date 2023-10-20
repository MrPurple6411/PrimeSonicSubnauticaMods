namespace CyclopsEngineUpgrades.Craftables;

using System.IO;
using System.Reflection;
using MoreCyclopsUpgrades.API.Upgrades;
using Nautilus.Crafting;
using static CraftData;

internal class PowerUpgradeModuleMk2 : CyclopsUpgrade
{
    public PowerUpgradeModuleMk2() 
        : base("PowerUpgradeModuleMk2",
              "Cyclops Engine Efficiency Module MK2",
              "Additional enhancement to engine efficiency.\n" +
              "Silent running, Sonar, and Shield optimized.\n" +
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
                new Ingredient(TechType.PowerUpgradeModule, 1),
                new Ingredient(TechType.Kyanite, 1),
                new Ingredient(TechType.Magnetite, 2)
            }
        };
    }
}
