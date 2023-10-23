namespace UpgradedVehicles;

using System.Collections.Generic;
using Nautilus.Crafting;
using static CraftData;

internal class SpeedBoosterMK4 : VehicleUpgradeModule
{
    public SpeedBoosterMK4()
        : base(classId: "SpeedModuleMK4",
            friendlyName: "Speed Boost Module MK4",
            description: "Speed Boost of 4 modules with the efficiency loss of 1")
    {
        CustomPrefab.AddOnRegister(() =>
        {
            VehicleUpgrader.CommonUpgradeModules.Add(Info.TechType);
            VehicleUpgrader.SpeedBoostingModules.Add(Info.TechType, 4);
        });
    }

    protected override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
    protected override string[] StepsToFabricatorTab => new[] { Plugin.WorkBenchSpeedTab };

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>
            {
                new Ingredient(Plugin.SpeedBoosterMK3.CustomPrefab.Info.TechType, 1),
                new Ingredient(TechType.Nickel, 1),
                new Ingredient(TechType.PrecursorIonCrystal, 2),
            }
        };
    }
}