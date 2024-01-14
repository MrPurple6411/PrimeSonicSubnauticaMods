namespace UpgradedVehicles.Modules.Speed;

using Nautilus.Crafting;
using UpgradedVehicles.Handlers;
using UpgradedVehicles.Modules;

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
            VehicleUpgradeHandler.RegisterSpeedModule(Info.TechType, 1);
        });
    }

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new ()
            {
                new (TechType.Titanium, 2),
                new (TechType.WiringKit, 1),
                new (TechType.ComputerChip, 1),
            }
        };
    }
}