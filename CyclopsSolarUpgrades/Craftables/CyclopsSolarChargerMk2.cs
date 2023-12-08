namespace CyclopsSolarUpgrades.Craftables;

using System.Collections;
using System.IO;
using System.Reflection;
using MoreCyclopsUpgrades.API.Upgrades;
using Nautilus.Crafting;
using UnityEngine;
using static CraftData;

internal class CyclopsSolarChargerMk2 : CyclopsUpgrade
{
    internal const float BatteryCapacity = 120f;

    public static CyclopsSolarCharger PreviousTier { get; internal set; }

    public CyclopsSolarChargerMk2()
        : base("CyclopsSolarChargerMk2",
               "Cyclops Solar Charger Mk2",
               "Improved solar charging for the Cyclops with additional backup power.\nStacks with other solar chargers.")
    {
    }

    public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Workbench;
    public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
    public override string[] StepsToFabricatorTab { get; } = new[] { "CyclopsMenu" };
    public override TechType RequiredForUnlock => TechType.SeamothSolarCharge;

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            craftAmount = 1,
            Ingredients =
            {
                new Ingredient(PreviousTier.TechType, 1),
                new Ingredient(TechType.Battery, 2),
                new Ingredient(TechType.WiringKit, 1)
            }
        };
    }

    public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
    {

        var task = new TaskResult<GameObject>();
        yield return base.GetGameObjectAsync(task);
        GameObject obj = task.Get();

        Battery pCell = obj.AddComponent<Battery>();
        pCell.name = "SolarBackupBattery2";
        pCell._capacity = BatteryCapacity;


        gameObject.Set(obj);
    }
}
