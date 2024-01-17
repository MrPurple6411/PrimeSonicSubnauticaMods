namespace UpgradedVehicles.Modules.Power;

using Nautilus.Crafting;
using UpgradedVehicles.Handlers;

internal class PowerEfficiencyMK4 : VehicleUpgradeModule
{
    private const int Tier = 4;

    public PowerEfficiencyMK4() : 
        base(classId: $"{TechType.VehiclePowerUpgradeModule.AsString()}MK4",
            friendlyName: "Engine Efficiency Module MK4",
            description: "Equivilent to 4 regular Engine Efficiency Modules")
    {
        CustomPrefab.AddOnRegister(() =>
        {
            VehicleUpgradeHandler.RegisterPowerEfficiencyModule(Info.TechType, Tier);
        });
    }

    protected override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
    protected override string[] StepsToFabricatorTab => new[] { Plugin.WorkBenchPowerTab };
    protected override TechType PrefabTemplateType => TechType.VehiclePowerUpgradeModule;

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new()
            {
                new (Plugin.PowerEfficiencyMK3.Info.TechType, 1),
                new (TechType.ComputerChip, 1),
                new (TechType.AdvancedWiringKit, 1),
            }
        };
    }
}
