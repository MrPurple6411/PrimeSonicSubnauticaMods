namespace UpgradedVehicles.SaveData;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Common;
using EasyMarkup;
using UpgradedVehicles.Handlers;

internal class ConfigSaveData : EmPropertyCollection
{
    internal bool Initialized { get; private set; }

    internal static readonly string[] SpeedSettingLabels = new[]
    {
        BonusSpeedStyles.Disabled.ToString(),
        BonusSpeedStyles.Slower.ToString(),
        BonusSpeedStyles.Normal.ToString(),
        BonusSpeedStyles.Faster.ToString()
    };

    internal const string ConfigKey = "UpgradedVehiclesOptions";
    internal const string BonusSpeedSuffix = "BonusSpeed";
    internal const string EnableDebugLogsID = "EnableDebugLogging";

    private static readonly string ConfigDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    private static string ConfigFile => Path.Combine(ConfigDirectory, $"{ConfigKey}.txt");

    internal bool ValidDataRead = true;

    internal bool DebugLogsEnabled
    {
        get => EmDebugLogs.Value;
        set => EmDebugLogs.Value = value;
    }

    private readonly EmProperty<bool> EmDebugLogs;

    internal BonusSpeedStyles? GetBonusSpeedStyle(TechType techType)
    {
        if (Properties.TryGetValue(techType.AsString() + BonusSpeedSuffix, out EmProperty emProperty))
            return (BonusSpeedStyles)((EmProperty<BonusSpeedStyles>)emProperty)?.Value;

        return null;
    }

    internal void SetBonusSpeedStyle(TechType techType, BonusSpeedStyles speedStyle)
    {
        if (Properties.TryGetValue(techType.AsString() + BonusSpeedSuffix, out EmProperty emProperty))
        {
            if (emProperty is EmProperty<BonusSpeedStyles> speedStyleProperty)
                speedStyleProperty.Value = speedStyle;
        }
        else
        {
            emProperty = new EmProperty<BonusSpeedStyles>(techType.AsString() + BonusSpeedSuffix, speedStyle);
            Properties[techType.AsString() + BonusSpeedSuffix] = emProperty;
            Definitions.Add(emProperty);
        }
    }

    internal void SetBonusSpeedStyles(Dictionary<TechType, BonusSpeedStyles> speedStyles)
    {
        foreach (var kvp in speedStyles)
        {
            if (Properties.TryGetValue(kvp.Key.AsString() + BonusSpeedSuffix, out EmProperty emProperty))
            {
                if (emProperty is EmProperty<BonusSpeedStyles> speedStyleProperty)
                    speedStyleProperty.Value = kvp.Value;
            }
            else
            {
                emProperty = new EmProperty<BonusSpeedStyles>(kvp.Key.AsString() + BonusSpeedSuffix, kvp.Value);
                Properties[kvp.Key.AsString() + BonusSpeedSuffix] = emProperty;
                Definitions.Add(emProperty);
            }
        }
    }
    
    private static ICollection<EmProperty> SaveDataDefinitions => new List<EmProperty>()
    {
        new EmProperty<bool>(EnableDebugLogsID, false){ Optional = true }
    };

    public ConfigSaveData() : base(ConfigKey, SaveDataDefinitions)
    {
        EmDebugLogs = (EmProperty<bool>)Properties[EnableDebugLogsID];
        OnValueExtractedEvent += IsReadDataValid;
    }

    public ConfigSaveData(Dictionary<TechType, BonusSpeedStyles> currentValues) : this()
    {
        foreach (var kvp in currentValues)
        {
            if (!Properties.TryGetValue(kvp.Key.AsString() + BonusSpeedSuffix, out var emProperty))
                emProperty = new EmProperty<BonusSpeedStyles>(kvp.Key.AsString() + BonusSpeedSuffix, BonusSpeedStyles.Normal);

            if (emProperty is EmProperty<BonusSpeedStyles> speedStyleProperty)
               speedStyleProperty.Value = kvp.Value;

            Properties[kvp.Key.AsString()+ BonusSpeedSuffix] = emProperty;
            Definitions.Add(emProperty);
        }
    }

    private void IsReadDataValid()
    {
        ValidDataRead = true;

        switch (this.GetBonusSpeedStyle(TechType.Seamoth))
        {
            case BonusSpeedStyles.Disabled:
            case BonusSpeedStyles.Slower:
            case BonusSpeedStyles.Normal:
            case BonusSpeedStyles.Faster:
                break;
            default:
                ValidDataRead &= false;
                break;
        }

        switch (this.GetBonusSpeedStyle(TechType.Exosuit))
        {
            case BonusSpeedStyles.Disabled:
            case BonusSpeedStyles.Slower:
            case BonusSpeedStyles.Normal:
            case BonusSpeedStyles.Faster:
                break;
            default:
                ValidDataRead &= false;
                break;
        }
    }

    public override EmProperty Copy()
    {
        Dictionary<TechType, BonusSpeedStyles> currentValues = new Dictionary<TechType, BonusSpeedStyles>();
        foreach (var property in this.Properties)
        {
            if (property.Value is EmProperty<BonusSpeedStyles> speedStyleProperty && TechTypeExtensions.FromString(property.Key.Replace(BonusSpeedSuffix, string.Empty), out var techType, true))
                currentValues.Add(techType, speedStyleProperty.Value);
        }

        return new ConfigSaveData(currentValues);
    }

    internal void InitializeSaveFile()
    {
        if (Initialized)
            return;

        foreach (var techType in VehicleUpgradeHandler.GetVehicleTypes())
        {
            SetBonusSpeedStyle(techType, BonusSpeedStyles.Normal);
        }

        try
        {
            Load();
        }
        catch (Exception ex)
        {
            QuickLogger.Info($"Error loading {ConfigKey}: " + ex.ToString());
            Save();
        }
        Initialized = true;
    }

    public void Save()
    {
        File.WriteAllLines(ConfigFile, new[]
        {
            "# -------------------------------------------------------------------- #",
            "# This config file can be edited in-game through the Mods options menu #",
            "#             This config file was built using EasyMarkup              #",
            "# -------------------------------------------------------------------- #",
            "",
            PrettyPrint()
        },
        Encoding.UTF8);
    }

    private void Load()
    {
        if (!File.Exists(ConfigFile))
        {
            QuickLogger.Info($"Config file not found. Writing default file.");
            Save();
            return;
        }

        string text = File.ReadAllText(ConfigFile, Encoding.UTF8);

        bool readCorrectly = FromString(text);

        if (!readCorrectly || !ValidDataRead)
        {
            QuickLogger.Info($"Config file contained errors. Writing default file.");
            Save();
        }
    }

    internal Dictionary<string, BonusSpeedStyles> GetBonusSpeedStyles()
    {
        Dictionary<string, BonusSpeedStyles> speedStyles = new Dictionary<string, BonusSpeedStyles>();

        foreach (var property in Properties)
        {
            if (property.Value is EmProperty<BonusSpeedStyles> speedStyleProperty)
                speedStyles.Add(property.Key, speedStyleProperty.Value);
        }

        return speedStyles;
    }
}
