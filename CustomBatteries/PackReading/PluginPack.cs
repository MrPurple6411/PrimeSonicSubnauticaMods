﻿namespace CustomBatteries.PackReading
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using CustomBatteries.API;

    internal class PluginPack : EmPropertyCollection, IPluginDetails
    {
        private static ICollection<EmProperty> PluginDefinitions => new List<EmProperty>
        {
            new EmProperty<string>(nameof(IPluginPack.PluginPackName)),
            new EmProperty<int>(nameof(IPluginPack.BatteryCapacity)),
            new EmProperty<TechType>(nameof(IPluginPack.UnlocksWith), TechType.None){ Optional = true },
            new EmProperty<string>(nameof(IPluginPack.BatteryID)),
            new EmProperty<string>(nameof(IPluginPack.BatteryName)),
            new EmProperty<string>(nameof(IPluginPack.BatterFlavorText)),
            new EmPropertyList<TechType>(nameof(IPluginPack.BatteryParts)),
            new EmProperty<string>(nameof(IPluginPack.BatteryIconFile)),
            new EmProperty<string>(nameof(IPluginPack.PowerCellID)),
            new EmProperty<string>(nameof(IPluginPack.PowerCellName)),
            new EmProperty<string>(nameof(IPluginPack.PowerCellFlavorText)),
            new EmPropertyList<TechType>(nameof(IPluginPack.PowerCellAdditionalParts)){ Optional = true },
            new EmProperty<string>(nameof(IPluginPack.PowerCellIconFile))
        };

        private readonly EmProperty<string> pluginPackName;
        private readonly EmProperty<int> batteryCapacity;
        private readonly EmProperty<TechType> unlocksWith;
        private readonly EmProperty<string> batteryID;
        private readonly EmProperty<string> batteryName;
        private readonly EmProperty<string> batterFlavorText;
        private readonly EmPropertyList<TechType> batteryParts;
        private readonly EmProperty<string> batteryIconFile;
        private readonly EmProperty<string> powerCellID;
        private readonly EmProperty<string> powerCellName;
        private readonly EmProperty<string> powerCellFlavorText;
        private readonly EmPropertyList<TechType> powerCellAdditionalParts;
        private readonly EmProperty<string> powerCellIconFile;

        public PluginPack()
            : base("CustomBatteriesPack", PluginDefinitions)
        {
            pluginPackName = (EmProperty<string>)Properties[nameof(IPluginPack.PluginPackName)];
            batteryCapacity = (EmProperty<int>)Properties[nameof(IPluginPack.BatteryCapacity)];
            unlocksWith = (EmProperty<TechType>)Properties[nameof(IPluginPack.UnlocksWith)];
            batteryID = (EmProperty<string>)Properties[nameof(IPluginPack.BatteryID)];
            batteryName = (EmProperty<string>)Properties[nameof(IPluginPack.BatteryName)];
            batterFlavorText = (EmProperty<string>)Properties[nameof(IPluginPack.BatterFlavorText)];
            batteryParts = (EmPropertyList<TechType>)Properties[nameof(IPluginPack.BatteryParts)];
            batteryIconFile = (EmProperty<string>)Properties[nameof(IPluginPack.BatteryIconFile)];
            powerCellID = (EmProperty<string>)Properties[nameof(IPluginPack.PowerCellID)];
            powerCellName = (EmProperty<string>)Properties[nameof(IPluginPack.PowerCellName)];
            powerCellFlavorText = (EmProperty<string>)Properties[nameof(IPluginPack.PowerCellFlavorText)];
            powerCellAdditionalParts = (EmPropertyList<TechType>)Properties[nameof(IPluginPack.PowerCellAdditionalParts)];
            powerCellIconFile = (EmProperty<string>)Properties[nameof(IPluginPack.PowerCellIconFile)];
        }

        private PluginPack(IPluginPack pluginPack)
            : this()
        {
            this.PluginPackName = pluginPack.PluginPackName;
            this.BatteryCapacity = pluginPack.BatteryCapacity;
            this.UnlocksWith = pluginPack.UnlocksWith;
            this.BatteryID = pluginPack.BatteryID;
            this.BatteryName = pluginPack.BatteryName;
            this.BatterFlavorText = pluginPack.BatterFlavorText;
            this.BatteryParts = pluginPack.BatteryParts;
            this.BatteryIconFile = pluginPack.BatteryIconFile;
            this.PowerCellID = pluginPack.PowerCellID;
            this.PowerCellName = pluginPack.PowerCellName;
            this.PowerCellFlavorText = pluginPack.PowerCellFlavorText;
            this.PowerCellAdditionalParts = pluginPack.PowerCellAdditionalParts;
            this.PowerCellIconFile = pluginPack.PowerCellIconFile;
        }

        public string PluginPackName
        {
            get => pluginPackName.Value;
            private set => pluginPackName.Value = value;
        }
        public int BatteryCapacity
        {
            get => batteryCapacity.Value;
            private set => batteryCapacity.Value = value;
        }
        public TechType UnlocksWith
        {
            get => unlocksWith.Value;
            private set => unlocksWith.Value = value;
        }
        public string BatteryID
        {
            get => batteryID.Value;
            private set => batteryID.Value = value;
        }
        public string BatteryName
        {
            get => batteryName.Value;
            private set => batteryName.Value = value;
        }
        public string BatterFlavorText
        {
            get => batterFlavorText.Value;
            private set => batterFlavorText.Value = value;
        }
        public IList<TechType> BatteryParts
        {
            get => batteryParts.Values;
            private set
            {
                batteryParts.Values.Clear();
                foreach (TechType item in value)
                    batteryParts.Values.Add(item);
            }
        }
        public string BatteryIconFile
        {
            get => batteryIconFile.Value;
            private set => batteryIconFile.Value = value;
        }
        public string PowerCellID
        {
            get => powerCellID.Value;
            private set => powerCellID.Value = value;
        }
        public string PowerCellName
        {
            get => powerCellName.Value;
            private set => powerCellName.Value = value;
        }
        public string PowerCellFlavorText
        {
            get => powerCellFlavorText.Value;
            private set => powerCellFlavorText.Value = value;
        }
        public IList<TechType> PowerCellAdditionalParts
        {
            get => powerCellAdditionalParts.Values;
            private set
            {
                powerCellAdditionalParts.Values.Clear();
                foreach (TechType item in value)
                    powerCellAdditionalParts.Values.Add(item);
            }
        }
        public string PowerCellIconFile
        {
            get => powerCellIconFile.Value;
            private set => powerCellIconFile.Value = value;
        }

        public string PluginPackFolder { get; set; }

        internal override EmProperty Copy()
        {
            return new PluginPack(this);
        }
    }
}
