namespace UpgradedVehicles.Modules.Power;

using Nautilus.Crafting;
using UpgradedVehicles.Handlers;

internal class PowerEfficiencyMK2 : VehicleUpgradeModule
{
    private const int Tier = 2;

    public PowerEfficiencyMK2() : 
        base(classId: $"{TechType.VehiclePowerUpgradeModule.AsString()}MK2",
            friendlyName: "Engine Efficiency Module MK2",
            description: "Double the efficiency of the basic Engine Efficiency Module")
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
                new (TechType.VehiclePowerUpgradeModule, 1),
                new (TechType.ComputerChip, 1),
                new (TechType.AdvancedWiringKit, 1),
            }
        };
    }
}
