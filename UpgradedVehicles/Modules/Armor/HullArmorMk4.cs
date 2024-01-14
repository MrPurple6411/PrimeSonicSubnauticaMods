namespace UpgradedVehicles.Modules.Armor;

using Nautilus.Crafting;
using UpgradedVehicles.Handlers;
using UpgradedVehicles.Modules;

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
            VehicleUpgradeHandler.RegisterArmorPlatingModule(Info.TechType, ArmorCount);
        });
    }

    protected override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;
    protected override string[] StepsToFabricatorTab => new[] { Plugin.WorkBenchArmorTab };

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            craftAmount = 1,
            Ingredients = new ()
            {
                new (Plugin.HullArmorMk3.Info.TechType, 1),
                new (TechType.Titanium, 4),
                new (TechType.Nickel, 1),
                new (TechType.ComputerChip, 1)
            }
        };
    }
}