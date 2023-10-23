namespace UpgradedVehicles;

using System.Collections.Generic;
using Nautilus.Crafting;
using static CraftData;

internal class SpeedBoosterMK2 : VehicleUpgradeModule
{
    public SpeedBoosterMK2()
        : base(classId: "SpeedModuleMK2",
            friendlyName: "Speed Boost Module MK2",
            description: "Speed Boost of 2 modules with the efficiency loss of 1")
    {
        CustomPrefab.AddOnRegister(() =>
        {
            VehicleUpgrader.CommonUpgradeModules.Add(Info.TechType);
            VehicleUpgrader.SpeedBoostingModules.Add(Info.TechType, 2);
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
                new Ingredient(Plugin.SpeedBooster.CustomPrefab.Info.TechType, 1),
                new Ingredient(TechType.Lithium, 1),
                new Ingredient(TechType.AcidMushroom, 2),
            }
        };
    }
}