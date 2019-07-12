﻿namespace CyclopsThermalUpgrades.Management
{
    using CommonCyclopsUpgrades;

    internal class ThermalCharger : AmbientEnergyCharger<ThermalUpgradeHandler>
    {
        private const float ThermalChargingFactor = 1.5f;

        public ThermalCharger(TechType tier2Id2, SubRoot cyclops)
            : base(TechType.CyclopsThermalReactorModule, tier2Id2, cyclops)
        {
        }

        protected override string PercentNotation => "°C";
        protected override float MaximumEnergyStatus => 100f;
        protected override float MinimumEnergyStatus => 35f;

        protected override void UpdateEnergyStatus(ref float ambientEnergyStatus)
        {
            if (WaterTemperatureSimulation.main == null)
            {
                ambientEnergyStatus = 0f; // Safety check
                return;
            }

            ambientEnergyStatus = WaterTemperatureSimulation.main.GetTemperature(base.Cyclops.transform.position);
        }

        protected override float ConvertToAvailableEnergy(float energyStatus)
        {
            // This is based on the original Cyclops thermal charging code
            return ThermalChargingFactor *
                   DayNightCycle.main.deltaTime *
                   base.Cyclops.thermalReactorCharge.Evaluate(energyStatus); // Temperature
        }
    }
}
