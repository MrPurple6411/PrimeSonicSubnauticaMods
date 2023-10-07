namespace CustomCraft3;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using Common;
using CustomCraft3.Serialization;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
public class Plugin: BaseUnityPlugin
{
    private static readonly string version = QuickLogger.GetAssemblyVersion();

    static Plugin()
    {
        QuickLogger.Info($"Setting up logging. Version {version}");

        if (!Directory.Exists(FileLocations.AssetsFolder))
            _ = Directory.CreateDirectory(FileLocations.AssetsFolder);

        if (!Directory.Exists(FileLocations.WorkingFolder))
            _ = Directory.CreateDirectory(FileLocations.WorkingFolder);

        CustomCraft2Config.CheckLogLevel();
    }

    public void Awake()
    {
        QuickLogger.Info($"Restoring files. Version {version}");

        try
        {
            RestoreAssets();
            HelpFilesWriter.HandleHelpFiles();
        }
        catch
        {
            QuickLogger.Error($"Critical error during file restoration");
            throw;
        }
    }

    public IEnumerator Start()
    {
        QuickLogger.Info($"Started patching. Version {version}");

        try
        {
            WorkingFileParser.HandleWorkingFiles();

            QuickLogger.Info("Finished patching.");
        }
        catch
        {
            QuickLogger.Error($"Critical error during file patching");
            throw;
        }

        yield break;
    }

    internal static void RestoreAssets()
    {
        string prefix = "CustomCraft3.Assets.";

        var ass = Assembly.GetExecutingAssembly();
        IEnumerable<string> resources = ass.GetManifestResourceNames().Where(name => name.StartsWith(prefix));

        foreach (string resource in resources)
        {
            string file = resource[(resource[..resource.LastIndexOf(".")].LastIndexOf(".") + 1)..];

            if (!Directory.Exists(FileLocations.AssetsFolder))
                _ = Directory.CreateDirectory(FileLocations.AssetsFolder);

            string outFile = Path.Combine(FileLocations.AssetsFolder, file);
            if (!File.Exists(outFile))
            {
                QuickLogger.Debug($"Restoring asset: {file}");

                Stream s = ass.GetManifestResourceStream(resource);
                var r = new BinaryReader(s);
                File.WriteAllBytes(outFile, r.ReadBytes((int)s.Length));
            }
        }
    }
}
