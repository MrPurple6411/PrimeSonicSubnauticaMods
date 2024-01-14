namespace UpgradedVehicles.Modules.Speed;

using Nautilus.Crafting;
using UpgradedVehicles.Handlers;
using UpgradedVehicles.Modules;

internal class SpeedBoosterMK3 : VehicleUpgradeModule
{
    public SpeedBoosterMK3()
        : base(classId: "SpeedModuleMK3",
            friendlyName: "Speed Boost Module MK3",
            description: "Speed Boost of 3 modules with the efficiency loss of 1")
    {
        CustomPrefab.AddOnRegister(() =>
        {
            VehicleUpgradeHandler.RegisterSpeedModule(Info.TechType, 3);
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
                new (Plugin.SpeedBoosterMK2.Info.TechType, 1),
                new (TechType.AluminumOxide, 1),
#if SUBNAUTICA
                new (TechType.WhiteMushroom, 2),
#elif BELOWZERO
                new (TechType.KelpRootPustule, 2),
#endif
            }
        };
    }
}