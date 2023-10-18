namespace Common;

using System;
using System.Reflection;
using BepInEx.Logging;

internal static class QuickLogger
{
    private static readonly AssemblyName ModName = Assembly.GetExecutingAssembly().GetName();
    private static readonly ManualLogSource _manualLogSource;

    internal static bool DebugLogsEnabled = false;

    static QuickLogger()
    {
        _manualLogSource = Logger.CreateLogSource(ModName.Name);
    }

    public static void Info(string msg, bool showOnScreen = false)
    {
        _manualLogSource.LogInfo(msg);
        if (showOnScreen)
            ErrorMessage.AddMessage(msg);
    }

    public static void Debug(string msg, bool showOnScreen = false)
    {
        _manualLogSource.LogDebug(msg);

        if (DebugLogsEnabled && showOnScreen)
            ErrorMessage.AddDebug(msg);
    }

    public static void Error(string msg, bool showOnScreen = false)
    {
        _manualLogSource.LogError(msg);

        if (showOnScreen)
            ErrorMessage.AddError(msg);
    }

    public static void Error(string msg, Exception ex)
    {
        _manualLogSource.LogError($"{ msg}{ Environment.NewLine}{ex}");
    }

    public static void Error(Exception ex)
    {
        _manualLogSource.LogError(ex);
    }

    public static void Warning(string msg, bool showOnScreen = false)
    {
        _manualLogSource.LogWarning(msg);

        if (showOnScreen)
            ErrorMessage.AddWarning(msg);
    }

    /// <summary>
    /// Creates the version string in format "#.#.#" or "#.#.# rev:#"
    /// </summary>
    public static string GetAssemblyVersion()
    {
        Version version = ModName.Version;

        //      Major Version
        //      Minor Version
        //      Build Number
        //      Revision

        if (version.Revision > 0)
        {
            return $"{version.Major}.{version.Minor}.{version.Build} rev:{version.Revision}";
        }

        if (version.Build > 0)
        {
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        if (version.Minor > 0)
        {
            return $"{version.Major}.{version.Minor}.0";
        }

        return $"{version.Major}.0.0";
    }

    public static string GetAssemblyName()
    {
        return ModName.Name;
    }
}
