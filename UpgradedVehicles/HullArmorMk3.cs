namespace UpgradedVehicles;

using System;
using System.Collections.Generic;
using Nautilus.Crafting;
using static CraftData;

internal class HullArmorMk3 : VehicleUpgradeModule
{
    private const int ArmorCount = 3;

    public HullArmorMk3()
        : base(classId: "HullArmorMk3",
            friendlyName: "Hull Reinforcement Mk III",
            description: "An upgrade containing nanites improving and maintaining the inner structure of the hull.\nEquivalent to 3 regular Hull Reinforcements")
    {
        CustomPrefab.AddOnRegister(() =>
        {
            VehicleUpgrader.CommonUpgradeModules.Add(Info.TechType);
            VehicleUpgrader.ArmorPlatingModules.Add(Info.TechType, ArmorCount);
        });
    }

    protected override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
    protected override string[] StepsToFabricatorTab => new[] { Plugin.WorkBenchTab };

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>
            {
                new Ingredient(Plugin.HullArmorMk2.Info.TechType, 1),
                new Ingredient(TechType.Titanium, 3),
                new Ingredient(TechType.AluminumOxide, 1),
                new Ingredient(TechType.ComputerChip, 1)
            }
        };
    }
}