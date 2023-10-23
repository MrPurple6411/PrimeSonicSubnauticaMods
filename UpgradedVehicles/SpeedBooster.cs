namespace UpgradedVehicles;

using System;
using System.Collections.Generic;
using Nautilus.Crafting;
using static CraftData;
#if SUBNAUTICA
using Sprite = Atlas.Sprite;
#endif

internal class SpeedBooster : VehicleUpgradeModule
{
    public static TechType TechType { get; private set; }

    public SpeedBooster()
        : base(classId: "SpeedModule",
            friendlyName: "Speed Boost Module",
            description: "Allows small vehicle engines to go into overdrive, adding greater speeds but at the cost of higher energy consumption rates.")
    {
        CustomPrefab.AddOnRegister(() =>
        {
            VehicleUpgrader.CommonUpgradeModules.Add(Info.TechType);
            VehicleUpgrader.SpeedBoostingModules.Add(Info.TechType, 1);
        });
    }

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>
            {
                new Ingredient(TechType.Titanium, 2),
                new Ingredient(TechType.WiringKit, 1),
                new Ingredient(TechType.ComputerChip, 1),
            }
        };
    }
}