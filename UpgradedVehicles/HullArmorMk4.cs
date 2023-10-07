namespace UpgradedVehicles;

using System;
using System.Collections.Generic;
using Nautilus.Crafting;
using static CraftData;

internal class HullArmorMk4 : VehicleUpgradeModule
{
    private const int ArmorCount = 4;
    public HullArmorMk4()
        : base(classId: "HullArmorMk4",
            friendlyName: "Hull Reinforcement Mk IV",
            description: "An upgrade containing nanites improving and maintaining the inner structure of the hull.\nEquivalent to 4 regular Hull Reinforcements")
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
                new Ingredient(Plugin.HullArmorMk3.Info.TechType, 1),
                new Ingredient(TechType.Titanium, 4),
                new Ingredient(TechType.Nickel, 1),
                new Ingredient(TechType.ComputerChip, 1)
            }
        };
    }
}