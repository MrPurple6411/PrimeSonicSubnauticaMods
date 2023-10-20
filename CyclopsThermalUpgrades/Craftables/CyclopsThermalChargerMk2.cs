namespace CyclopsThermalUpgrades.Craftables;

using System.Collections;
using System.IO;
using System.Reflection;
using MoreCyclopsUpgrades.API.Upgrades;
using Nautilus.Crafting;
using Nautilus.Handlers;
using UnityEngine;
using static CraftData;

internal class CyclopsThermalChargerMk2 : CyclopsUpgrade
{
    internal const float BatteryCapacity = 120f;

    private const string MaxThermalReachedKey = "MaxThermalMsg";
    internal static string MaxThermalReached()
    {
        return Language.main.Get(MaxThermalReachedKey);
    }

    public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Workbench;
    public override string[] StepsToFabricatorTab { get; } = new[] { "Vanilla" };
    public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
    public override TechType RequiredForUnlock { get; } = TechType.Workbench;
    public override TechType SortAfter { get; } = TechType.CyclopsThermalReactorModule;

    public CyclopsThermalChargerMk2()
        : base("CyclopsThermalChargerMk2",
               "Cyclops Thermal Reactor Mk2",
               "Improved thermal charging with additional backup power.\nStacks with other thermal reactors.")
    {
        CustomPrefab.AddOnRegister(()=> LanguageHandler.SetLanguageLine(MaxThermalReachedKey, "Max number of thermal chargers reached."));
    }

    public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
    {
        var task = new TaskResult<GameObject>();
        yield return base.GetGameObjectAsync(task);
        GameObject obj = task.Get();

        Battery pCell = obj.AddComponent<Battery>();
        pCell.name = "ThermalBackupBattery";
        pCell._capacity = BatteryCapacity;

        gameObject.Set(obj);
    }

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            craftAmount = 1,
            Ingredients =
            {
                new Ingredient(TechType.CyclopsThermalReactorModule, 1),
                new Ingredient(TechType.Battery, 2),
                new Ingredient(TechType.WiringKit, 1)
            }
        };
    }
}
