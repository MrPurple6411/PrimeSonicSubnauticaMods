namespace UpgradedVehicles.Modules.Armor;

using Nautilus.Crafting;
using UpgradedVehicles.Handlers;
using UpgradedVehicles.Modules;

internal class HullArmorMk3 : VehicleUpgradeModule
{
    private const int ArmorCount = 3;

    public HullArmorMk3()
        : base(classId: "HullArmorMk3",
            friendlyName: "Hull Reinforcement Mk III",
            description: "An upgrade containing nanites improving and maintaining the inner structure of the hull.\nEquivalent to 3 regular Hull Reinforcements")
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
                new (Plugin.HullArmorMk2.Info.TechType, 1),
                new (TechType.Titanium, 3),
                new (TechType.AluminumOxide, 1),
                new (TechType.ComputerChip, 1)
            }
        };
    }
}