namespace CyclopsSimpleSolar;

using System.IO;
using System.Reflection;
using MoreCyclopsUpgrades.API;
using MoreCyclopsUpgrades.API.Upgrades;
using Nautilus.Crafting;
using static CraftData;

internal class CySolarModule : CyclopsUpgrade
{
    public CySolarModule() : base("CySimpSolarCharger",
               "Cyclops Solar Charging Module",
               "Recharges the Cyclops power cells while in sunlight.\n" +
               "DOES NOT STACK with other solar chargers.")
    {

    }

    public override CraftTree.Type FabricatorType => CraftTree.Type.CyclopsFabricator;

    public override string[] StepsToFabricatorTab => MCUServices.CrossMod.StepsToCyclopsModulesTabInCyclopsFabricator;

    public override TechType RequiredForUnlock => TechType.SeamothSolarCharge;

    public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

    protected override TechType PrefabTemplate => TechType.SeamothSolarCharge;

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            craftAmount = 1,
            Ingredients =
            {
                new Ingredient(TechType.AdvancedWiringKit, 1),
                new Ingredient(TechType.Glass, 1),
                new Ingredient(TechType.EnameledGlass, 1),
                new Ingredient(TechType.Titanium, 1),
            }
        };
    }
}
