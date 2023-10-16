﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CustomBatteries.API;
using Nautilus.Assets;
using UnityEngine;

namespace CustomBatteries.Items;

internal static class CbDatabase
{
    public const string BatteryCraftTab = "BatteryTab";
    public const string PowCellCraftTab = "PowCellTab";
    public const string ElecCraftTab = "Electronics";
    public const string ResCraftTab = "Resources";

    public static readonly string[] BatteryCraftPath = new[] { ResCraftTab, BatteryCraftTab };
    public static readonly string[] PowCellCraftPath = new[] { ResCraftTab, PowCellCraftTab };

    public static string ExecutingFolder { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    public static List<CbCore> BatteryItems { get; } = new List<CbCore>();
    public static List<CbCore> PowerCellItems { get; } = new List<CbCore>();

    public static Dictionary<TechType, CBModelData> BatteryModels { get; } = new Dictionary<TechType, CBModelData>();
    public static Dictionary<TechType, CBModelData> PowerCellModels { get; } = new Dictionary<TechType, CBModelData>();

    public static HashSet<TechType> TrackItems { get; } = new HashSet<TechType>();

    private static bool? _placeBatteriesFeatureEnabled = null;

    public static bool PlaceBatteriesFeatureEnabled
    {
        get
        {
            if (_placeBatteriesFeatureEnabled == null || !_placeBatteriesFeatureEnabled.HasValue)
            {
                if (BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue("com.osubmarin.decorationsmod", out var decorationsMod))
                {
                    Type decorationsModConfig = decorationsMod.GetType().Assembly.GetType("DecorationsMod.ConfigSwitcher", false);
                    if (decorationsModConfig != null)
                    {
                        FieldInfo enablePlaceBatteriesField = decorationsModConfig.GetField("EnablePlaceBatteries", BindingFlags.Public | BindingFlags.Static);
                        if (enablePlaceBatteriesField != null)
                            _placeBatteriesFeatureEnabled = (bool)enablePlaceBatteriesField.GetValue(null);
                    }
                }
            }
            return _placeBatteriesFeatureEnabled != null && _placeBatteriesFeatureEnabled.Value;
        }
    }

    private static GameObject _precursorionbattery;
    private static GameObject _precursorionpowercell;
    private static GameObject _battery;
    private static GameObject _powercell;

    public static GameObject IonBattery()
    {
        return _precursorionbattery ??= Resources.Load<GameObject>("worldentities/tools/precursorionbattery");
    }

    public static GameObject IonPowerCell()
    {
        return _precursorionpowercell ??= Resources.Load<GameObject>("worldentities/tools/precursorionpowercell");
    }

    public static GameObject Battery()
    {
        return _battery ??= Resources.Load<GameObject>("worldentities/tools/battery");
    }

    public static GameObject PowerCell()
    {
        return _powercell ??= Resources.Load<GameObject>("worldentities/tools/powercell");
    }
}
