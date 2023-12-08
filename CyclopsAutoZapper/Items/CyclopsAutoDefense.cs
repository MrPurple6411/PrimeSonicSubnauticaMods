﻿namespace CyclopsAutoZapper;

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CyclopsAutoZapper.Managers;
using MoreCyclopsUpgrades.API;
using MoreCyclopsUpgrades.API.Upgrades;
using Nautilus.Crafting;
using Nautilus.Handlers;
using static CraftData;

internal class CyclopsAutoDefense : CyclopsUpgrade
{
    public CyclopsAutoDefense()
        : base("CyclopsZapperModule",
               "Cyclops Auto Defense System Mk1",
               "Extends and automates the Perimeter Defense System of a docked Seamoth to protect the Cyclops from aggressive fauna.")
    {
        OnStartedPatching += () =>
        {
            KnownTechHandler.SetAnalysisTechEntry(TechType.SeamothElectricalDefense, new List<TechType>() { this.Info.TechType });
        };

        OnFinishedPatching += () =>
        {
            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            { return new UpgradeHandler(this.TechType, cyclops) { MaxCount = 1 }; });

            MCUServices.Register.PdaIconOverlay(this.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
            { return new AutoDefenseIconOverlay(icon, upgradeModule); });

            MCUServices.Register.AuxCyclopsManager<AutoDefenser>((SubRoot cyclops) =>
            { return new AutoDefenser(this.TechType, cyclops); });
        };
    }

    public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.CyclopsFabricator;
    public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
    public override TechType RequiredForUnlock { get; } = TechType.SeamothElectricalDefense;
    public override string[] StepsToFabricatorTab { get; } = MCUServices.CrossMod.StepsToCyclopsModulesTabInCyclopsFabricator;

    protected override RecipeData GetBlueprintRecipe()
    {
        return new RecipeData
        {
            craftAmount = 1,
            Ingredients =
            {
                new Ingredient(TechType.ComputerChip, 1),
                new Ingredient(TechType.WiringKit, 1),
                new Ingredient(TechType.CopperWire, 1),
                new Ingredient(TechType.Polyaniline, 1),
            }
        };
    }
}
