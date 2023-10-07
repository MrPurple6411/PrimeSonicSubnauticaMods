﻿namespace UpgradedVehicles.SaveData;

using Common;
using Nautilus.Handlers;
using Nautilus.Options;

internal class UpgradeOptions : ModOptions, IUpgradeOptions
{
    private readonly ConfigSaveData SaveData = new ConfigSaveData();

    public bool DebugLogsEnabled => SaveData.DebugLogsEnabled;

    public float SeamothBonusSpeedMultiplier
    {
        get
        {
            switch (SaveData.SeamothBonusSpeedSetting)
            {
                case BonusSpeedStyles.Disabled:
                    return 0f;
                case BonusSpeedStyles.Slower:
                    return 0.05f;
                case BonusSpeedStyles.Normal:
                    return 0.15f;
                case BonusSpeedStyles.Faster:
                    return 0.25f;
                default: // Error
                    return 0.125f;
            }
        }
    }

    public float ExosuitBonusSpeedMultiplier
    {
        get
        {
            switch (SaveData.ExosuitBonusSpeedSetting)
            {
                case BonusSpeedStyles.Disabled:
                    return 0f;
                case BonusSpeedStyles.Slower:
                    return 0.10f;
                case BonusSpeedStyles.Normal:
                    return 0.20f;
                case BonusSpeedStyles.Faster:
                    return 0.30f;
                default: // Error
                    return 0.15f;
            }
        }
    }

    internal int SeamothBonusSpeedIndex
    {
        get => (int)SaveData.SeamothBonusSpeedSetting;
        set => SaveData.SeamothBonusSpeedSetting = (BonusSpeedStyles)value;
    }

    internal int ExosuitBonusSpeedIndex
    {
        get => (int)SaveData.ExosuitBonusSpeedSetting;
        set => SaveData.ExosuitBonusSpeedSetting = (BonusSpeedStyles)value;
    }

    public UpgradeOptions() : base("Upgraded Vehicles Options")
    {
        var seamothBonusSpeed = ModChoiceOption<string>.Create(ConfigSaveData.SeamothBonusSpeedID, "Seamoth Bonus Speed", ConfigSaveData.SpeedSettingLabels, this.SeamothBonusSpeedIndex);
        seamothBonusSpeed.OnChanged += this.OnBonusSpeedStyleChanged;
        var exosuitBonusSpeed = ModChoiceOption<string>.Create(ConfigSaveData.ExosuitBonusSpeedID, "Prawn Suit Bonus Speed", ConfigSaveData.SpeedSettingLabels, this.ExosuitBonusSpeedIndex);
        exosuitBonusSpeed.OnChanged += this.OnBonusSpeedStyleChanged;

        AddItem(seamothBonusSpeed);
        AddItem(exosuitBonusSpeed);

    }

    public void Initialize()
    {
        QuickLogger.Info("Initializing save data and options");
        SaveData.InitializeSaveFile();
        OptionsPanelHandler.RegisterModOptions(this);
    }

    private void OnBonusSpeedStyleChanged(object sender, ChoiceChangedEventArgs<string> args)
    {
        switch (args.Id)
        {
            case ConfigSaveData.SeamothBonusSpeedID:
                this.SeamothBonusSpeedIndex = args.Index;
                break;

            case ConfigSaveData.ExosuitBonusSpeedID:
                this.ExosuitBonusSpeedIndex = args.Index;
                break;

            default:
                return;
        }

        VehicleUpgrader.SetBonusSpeedMultipliers(this);
        SaveData.Save();
    }
}
