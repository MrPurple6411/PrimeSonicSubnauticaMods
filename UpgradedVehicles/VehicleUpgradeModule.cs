namespace UpgradedVehicles;

using System.IO;
using System.Reflection;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;

public abstract class VehicleUpgradeModule
{
    internal PrefabInfo Info { get; init; }
    internal virtual CustomPrefab CustomPrefab { get; init; }
    protected virtual PrefabTemplate PrefabTemplate { get; init; }

    protected VehicleUpgradeModule(string classId, string friendlyName, string description)
    {
        Info = PrefabInfo.WithTechType(classId, friendlyName, description, "English", RequiredForUnlock == TechType.None).WithSizeInInventory(SizeInInventory);

        var iconPath = Path.Combine(AssetsFolder, $"{classId}.png");
        if (File.Exists(iconPath))
            Info.WithIcon(ImageUtils.LoadSpriteFromFile(iconPath));

        if (CustomPrefab == null)
            CustomPrefab = new CustomPrefab(Info);

        if (RequiredForUnlock != TechType.None)
            CustomPrefab.SetUnlock(RequiredForUnlock).WithAnalysisTech(null);
        
        CustomPrefab.SetPdaGroupCategory(GroupForPDA, CategoryForPDA);
        CustomPrefab.SetRecipe(GetBlueprintRecipe()).WithFabricatorType(FabricatorType).WithStepsToFabricatorTab(StepsToFabricatorTab);
        CustomPrefab.SetEquipment(EquipmentType.VehicleModule).WithQuickSlotType(QuickSlotType);

        if (PrefabTemplate == null)
            PrefabTemplate = new CloneTemplate(Info, PrefabTemplateType);

        CustomPrefab.SetGameObject(PrefabTemplate);
    }

    protected virtual Vector2int SizeInInventory { get; } = new(1, 1);
    protected TechGroup GroupForPDA => TechGroup.VehicleUpgrades;
    protected TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;
    protected virtual TechType PrefabTemplateType => TechType.SeamothSonarModule;
    protected virtual TechType RequiredForUnlock => TechType.BaseUpgradeConsole;
    protected virtual CraftTree.Type FabricatorType => CraftTree.Type.SeamothUpgrades;
    protected virtual QuickSlotType QuickSlotType => QuickSlotType.Passive;
    protected virtual string[] StepsToFabricatorTab { get; } = new[] { "CommonModules" };
    protected virtual string AssetsFolder { get; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

    protected virtual RecipeData GetBlueprintRecipe()
    {
        return CraftDataHandler.GetRecipeData(PrefabTemplateType);
    }
}