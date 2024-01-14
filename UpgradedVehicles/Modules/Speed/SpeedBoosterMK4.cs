namespace UpgradedVehicles.Modules.Speed;

using Nautilus.Crafting;
using UpgradedVehicles.Handlers;
using UpgradedVehicles.Modules;

internal class SpeedBoosterMK4 : VehicleUpgradeModule
{
    public SpeedBoosterMK4()
        : base(classId: "SpeedModuleMK4",
            friendlyName: "Speed Boost Module MK4",
            description: "Speed Boost of 4 modules with the efficiency loss of 1")
    {
        CustomPrefab.AddOnRegister(() =>
        {
            VehicleUpgradeHandler.RegisterSpeedModule(Info.TechType, 4);
        });
    }

    protected override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
    protected override string[] StepsToFabricatorTab => new[] { Plugin.WorkBenchSpeedTab };

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new ()
            {
                new (Plugin.SpeedBoosterMK3.Info.TechType, 1),
                new (TechType.Nickel, 1),
                new (TechType.PrecursorIonCrystal, 2),
            }
        };
    }
}