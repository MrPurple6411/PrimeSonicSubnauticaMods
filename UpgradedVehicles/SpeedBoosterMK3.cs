namespace UpgradedVehicles;

using System.Collections.Generic;
using Nautilus.Crafting;
using static CraftData;

internal class SpeedBoosterMK3 : VehicleUpgradeModule
{
    public SpeedBoosterMK3()
        : base(classId: "SpeedModuleMK3",
            friendlyName: "Speed Boost Module MK3",
            description: "Speed Boost of 3 modules with the efficiency loss of 1")
    {
        CustomPrefab.AddOnRegister(() =>
        {
            VehicleUpgrader.CommonUpgradeModules.Add(Info.TechType);
            VehicleUpgrader.SpeedBoostingModules.Add(Info.TechType, 3);
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
                new Ingredient(Plugin.SpeedBoosterMK2.CustomPrefab.Info.TechType, 1),
                new Ingredient(TechType.AluminumOxide, 1),
                new Ingredient(TechType.WhiteMushroom, 2),
            }
        };
    }
}