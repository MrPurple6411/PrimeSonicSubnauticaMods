namespace UpgradedVehicles.Modules.Speed;

using Nautilus.Crafting;
using UpgradedVehicles.Handlers;
using UpgradedVehicles.Modules;

internal class SpeedBoosterMK2 : VehicleUpgradeModule
{
    public SpeedBoosterMK2()
        : base(classId: "SpeedModuleMK2",
            friendlyName: "Speed Boost Module MK2",
            description: "Speed Boost of 2 modules with the efficiency loss of 1")
    {
        CustomPrefab.AddOnRegister(() =>
        {
            VehicleUpgradeHandler.RegisterSpeedModule(Info.TechType, 2);
        });
    }

    protected override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
    protected override string[] StepsToFabricatorTab => new[] { Plugin.WorkBenchSpeedTab };

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new()
            {
                new (Plugin.SpeedBooster.Info.TechType, 1),
                new (TechType.Lithium, 1),
#if SUBNAUTICA
                new (TechType.AcidMushroom, 2),
#elif BELOWZERO
                new (TechType.GenericRibbon, 2),
#endif
            }
        };
    }
}