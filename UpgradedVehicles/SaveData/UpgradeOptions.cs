namespace UpgradedVehicles.SaveData;

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Nautilus.Options;
using UpgradedVehicles.Handlers;

internal class UpgradeOptions : ModOptions
{
    public UpgradeOptions() : base(MyPluginInfo.PLUGIN_NAME)
    {
    }

    public override void BuildModOptions(uGUI_TabbedControlsPanel panel, int modsTabIndex, IReadOnlyCollection<OptionItem> options)
    {
        Plugin.SaveData.InitializeSaveFile();

        var optionsToAdd = new List<OptionItem>();
        foreach (var kvp in Plugin.SaveData.GetBonusSpeedStyles().OrderBy(o => o.Key))
        {
            bool exists = false;
            foreach (var option in options)
            {
                if (option.Id == kvp.Key)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists && TechTypeExtensions.FromString(kvp.Key.Replace(ConfigSaveData.BonusSpeedSuffix, string.Empty).Trim() , out var techType, true))
            {
                var prettyName = Language.main.GetOrFallback(techType, techType.AsString()) + " Bonus Speed";
                var option = ModChoiceOption<BonusSpeedStyles>.Create(kvp.Key, prettyName, (BonusSpeedStyles[])Enum.GetValues(typeof(BonusSpeedStyles)), (Plugin.SaveData.GetBonusSpeedStyle(techType) ?? BonusSpeedStyles.Normal));
                option.OnChanged += VehicleUpgradeHandler.OnBonusSpeedStyleChanged;
                optionsToAdd.Add(option);
            }
        }
        if (optionsToAdd.Count > 0)
            optionsToAdd.Do(o => AddItem(o));

        if (this.Options.Count > 0)
            base.BuildModOptions(panel, modsTabIndex, this.Options);
    }
}
