namespace UpgradedVehicles.Modules.Armor;

using Nautilus.Crafting;
using UpgradedVehicles.Handlers;
using UpgradedVehicles.Modules;

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
                new (TechType.VehicleArmorPlating, 1),
                new (TechType.Titanium, 2),
                new (TechType.Lead, 1),
                new (TechType.ComputerChip, 1)
            }
        };
    }
}