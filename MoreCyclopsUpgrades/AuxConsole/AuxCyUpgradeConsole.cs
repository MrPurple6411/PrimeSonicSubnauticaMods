namespace MoreCyclopsUpgrades.AuxConsole;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Common;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Extensions;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static CraftData;

internal static class AuxCyUpgradeConsole
{
    public static TechGroup GroupForPDA { get; } = TechGroup.InteriorModules;
    public static TechCategory CategoryForPDA { get; } = TechCategory.InteriorModule;
    public static string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
    public static TechType RequiredForUnlock { get; } = TechType.Cyclops;

    private const string OnHoverKey = "CyUpgradeOnHover";
    public static string OnHoverText => Language.main.Get(OnHoverKey);

    public static PrefabInfo Info { get; private set; }

    internal static void CreateAndRegister()
    {
        Info = PrefabInfo.WithTechType(classId: "AuxCyUpgradeConsole",
               displayName: "Auxiliary Upgrade Console",
               description: "A secondary upgrade console to connect a greater number of upgrades to your Cyclops.",
               language: "English", unlockAtStart: false)
            .WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "AuxCyUpgradeConsole.png")));

        var customPrefab = new CustomPrefab(Info);

        customPrefab.SetPdaGroupCategory(GroupForPDA, CategoryForPDA)
                .WithAnalysisTech(null, null, null)
                .RequiredForUnlock = RequiredForUnlock;

        customPrefab.SetRecipe(GetBlueprintRecipe());
        customPrefab.SetGameObject(GetGameObjectAsync);

        LanguageHandler.SetLanguageLine(OnHoverKey, "Use Auxiliary Cyclop Upgrade Console");
        customPrefab.Register();
    }

    private static RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            Ingredients = new List<Ingredient>
            {
                new Ingredient(TechType.AdvancedWiringKit, 1),
                new Ingredient(TechType.Titanium, 5),
                new Ingredient(TechType.Lithium, 1),
                new Ingredient(TechType.Lead, 1),
            }
        };
    }

    public static IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
    {
        QuickLogger.Debug("Preparing AuxCyUpgradeConsole prefab");

        var trashcanPrefabTask = CraftData.GetPrefabForTechTypeAsync(TechType.LabTrashcan);
        yield return trashcanPrefabTask;
        // The LabTrashcan prefab was chosen because it is very similar in size, shape, and collision model to the upgrade console model
        var obj = trashcanPrefabTask.GetResult();

        if (obj == null)
        {
            QuickLogger.Error("Failed to load the trashcan prefab");
            yield break;
        }

        var prefab = EditorModifications.Instantiate(obj, default, default, false);

        // Add the custom component
        AuxCyUpgradeConsoleMono auxConsole = prefab.AddComponent<AuxCyUpgradeConsoleMono>();
        var childIdentifier = prefab.GetComponentInChildren<ChildObjectIdentifier>(true);
        childIdentifier.transform.SetAsFirstSibling();
        childIdentifier.classId = Info.ClassID;
        auxConsole.modulesRoot = childIdentifier.gameObject;

        GameObject.DestroyImmediate(prefab.FindChild("discovery_trashcan_01_d")); // Don't need this
        GameObject.DestroyImmediate(prefab.GetComponent<Trashcan>()); // Don't need this
        GameObject.DestroyImmediate(prefab.GetComponent<StorageContainer>()); // Don't need this

        var task = new AssetReferenceGameObject("WorldEntities/Doodads/Debris/Wrecks/Decoration/submarine_engine_console_01_wide.prefab")
            .ForceValid().LoadAssetAsync();

        yield return task;

        if (task.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            QuickLogger.Error("Failed to load the console prefab");
            yield break;
        }

        GameObject consolePrefab = EditorModifications.Instantiate(task.Result, default, default, false);
        GameObject consoleWide = consolePrefab.FindChild("submarine_engine_console_01_wide");
        GameObject consoleModel = consoleWide.FindChild("console");

        // This is to tie the model to the prefab
        consoleModel.transform.SetParent(prefab.transform);
        consoleWide.SetActive(false);
        consolePrefab.SetActive(false);

        // Rotate to the correct orientation
        consoleModel.transform.rotation *= Quaternion.Euler(180f, 180f, 180f);

        // Update sky applier
        SkyApplier skyApplier = prefab.GetComponent<SkyApplier>();
        skyApplier.renderers = consoleModel.GetComponentsInChildren<MeshRenderer>();
        skyApplier.anchorSky = Skies.Auto;
        skyApplier.enabled = true;

        Constructable constructible = prefab.GetComponent<Constructable>();
        constructible.allowedInBase = false;
        constructible.allowedInSub = true; // Only allowed in Cyclops
        constructible.allowedOutside = false;
        constructible.allowedOnCeiling = false;
        constructible.allowedOnGround = true; // Only on ground
        constructible.allowedOnWall = false;
        constructible.allowedOnConstructables = false;
        constructible.controlModelState = true;
        constructible.rotationEnabled = true;
        constructible.techType = Info.TechType;
        constructible.model = consoleModel;

        gameObject.Set(prefab);
    }
}
