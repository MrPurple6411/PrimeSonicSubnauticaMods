namespace UpgradedVehicles.Modules;

using System.IO;
using System.Reflection;
using Common;
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
        if (TechTypeExtensions.FromString(classId, out _, true))
        {
            QuickLogger.Error($"TechType '{classId}' already exists, skipping");
            return;
        }

        Info = PrefabInfo.WithTechType(classId, friendlyName, description, "English", RequiredForUnlock == TechType.None)
            .WithSizeInInventory(SizeInInventory);

        var iconPath = Path.Combine(AssetsFolder, $"{classId}.png");
        if (File.Exists(iconPath))
            Info.WithIcon(ImageUtils.LoadSpriteFromFile(iconPath));
        else
            Info.WithIcon(SpriteManager.Get(PrefabTemplateType));

        CustomPrefab = new CustomPrefab(Info);

        if (RequiredForUnlock != TechType.None)
            CustomPrefab.SetUnlock(RequiredForUnlock).WithAnalysisTech(null, null, null);

        CustomPrefab.SetPdaGroupCategory(GroupForPDA, CategoryForPDA);
        CustomPrefab.SetRecipe(GetBlueprintRecipe()).WithFabricatorType(FabricatorType).WithStepsToFabricatorTab(StepsToFabricatorTab);
        CustomPrefab.SetEquipment(EquipmentType.VehicleModule).WithQuickSlotType(QuickSlotType);

        if (PrefabTemplate == null)
            PrefabTemplate = new CloneTemplate(Info, PrefabTemplateType) { ModifyPrefab = (go) => go.SetActive(false) };

        CustomPrefab.SetGameObject(PrefabTemplate);
    }

    protected virtual Vector2int SizeInInventory { get; } = new(1, 1);
    protected TechGroup GroupForPDA => TechGroup.VehicleUpgrades;
    protected TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;
    protected virtual TechType PrefabTemplateType => TechType.SeamothSonarModule;
    protected virtual TechType RequiredForUnlock => TechType.BaseUpgradeConsole;
    protected virtual QuickSlotType QuickSlotType => QuickSlotType.Passive;

    protected virtual CraftTree.Type FabricatorType =>
#if SUBNAUTICA
        CraftTree.Type.SeamothUpgrades;
#else
        CraftTree.Type.Fabricator;
#endif

    protected virtual string[] StepsToFabricatorTab { get; } = new[] {
#if SUBNAUTICA
        "CommonModules"
#else
        "Upgrades", "ExosuitUpgrades"
#endif
    };

    protected virtual string AssetsFolder { get; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

    protected virtual RecipeData GetBlueprintRecipe()
    {
        return CraftDataHandler.GetRecipeData(PrefabTemplateType);
    }
}