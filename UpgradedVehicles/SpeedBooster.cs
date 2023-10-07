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
    public SpeedBooster()
        : base(classId: "SpeedModule",
            friendlyName: "Speed Boost Module",
            description: "Allows small vehicle engines to go into overdrive, adding greater speeds but at the cost of higher energy consumption rates.")
    {
        CustomPrefab.AddOnRegister(() =>
        {
            VehicleUpgrader.CommonUpgradeModules.Add(Info.TechType);
            VehicleUpgrader.SpeedBoostingModule = Info.TechType;
        });
    }

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>
            {
                new Ingredient(TechType.Aerogel, 1),
                new Ingredient(TechType.Magnetite, 1),
                new Ingredient(TechType.Titanium, 2),
            }
        };
    }
}