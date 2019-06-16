﻿namespace CyclopsSolarUpgrades.Craftables
{
    using CyclopsSolarUpgrades.Management;
    using MoreCyclopsUpgrades.API;
    using SMLHelper.V2.Crafting;
    using UnityEngine;

    internal class CyclopsSolarChargerMk2 : CyclopsUpgrade
    {
        internal const float BatteryCapacity = 120f;

        private readonly CyclopsSolarCharger previousTier;
        public CyclopsSolarChargerMk2(CyclopsSolarCharger cyclopsSolarCharger)
            : base("CyclopsSolarChargerMk2",
                   "Cyclops Solar Charger Mk2",
                   "Improved solar charging and with integrated power storage for when you can't see the sun.\n" +
                  $"Stacks with other solar chargers up to a maximum of {Solar.MaxSolarChargers} total solar chargers.")
        {
            previousTier = cyclopsSolarCharger;
            OnStartedPatching += () =>
            {
                if (!previousTier.IsPatched)
                    previousTier.Patch();
            };

            OnFinishedPatching += () =>
            {
                Solar.CyclopsSolarChargerMk2ID = this.TechType;
            };
        }

        public override CraftTree.Type FabricatorType { get; } = CraftTree.Type.Workbench;
        public override string AssetsFolder { get; } = "CyclopsSolarUpgrades/Assets";
        public override string[] StepsToFabricatorTab { get; } = new[] { "CyclopsMenu" };

        protected override TechData GetBlueprintRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(previousTier.TechType, 1),
                    new Ingredient(TechType.PowerCell, 1),
                    new Ingredient(TechType.Benzene, 1),
                    new Ingredient(TechType.WiringKit, 1),
                    new Ingredient(TechType.CopperWire, 1),
                }
            };
        }

        public override GameObject GetGameObject()
        {
            GameObject obj = base.GetGameObject();

            Battery pCell = obj.AddComponent<Battery>();
            pCell.name = "SolarBackupBattery";
            pCell._capacity = BatteryCapacity;

            return obj;
        }


    }
}
