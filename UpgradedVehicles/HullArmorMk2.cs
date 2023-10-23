namespace UpgradedVehicles;

using System;
using System.Collections.Generic;
using Nautilus.Crafting;
using static CraftData;

internal class HullArmorMk2 : VehicleUpgradeModule
{
    private const int ArmorCount = 2;
    public HullArmorMk2()
        : base(classId: "HullArmorMk2",
            friendlyName: "Hull Reinforcement Mk II",
            description: "An upgrade containing nanites improving and maintaining the inner structure of the hull.\nEquivalent to 2 regular Hull Reinforcements")
    {
        CustomPrefab.AddOnRegister(() =>
        {
            VehicleUpgrader.CommonUpgradeModules.Add(Info.TechType);
            VehicleUpgrader.ArmorPlatingModules.Add(Info.TechType, ArmorCount);
        });
    }

    protected override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
    protected override string[] StepsToFabricatorTab => new[] { Plugin.WorkBenchArmorTab };

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>
            {
                new Ingredient(TechType.VehicleArmorPlating, 1),
                new Ingredient(TechType.Titanium, 2),
                new Ingredient(TechType.Lead, 1),
                new Ingredient(TechType.ComputerChip, 1)
            }
        };
    }
}