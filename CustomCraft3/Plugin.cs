#if !UNITY_EDITOR && (SUBNAUTICA || BELOWZERO)
namespace CustomCraft3
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using BepInEx;
    using Common;
    using CustomCraft3.Serialization;
    using UnityEngine;

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
    [BepInIncompatibility("com.ahk1221.smlhelper")]
#if SUBNAUTICA
    [BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
    public class Plugin : BaseUnityPlugin
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
            }
            catch
            {
                QuickLogger.Error($"Critical error during file restoration");
                throw;
            }
        }

        public IEnumerator Start()
        {
            while (Language.main is null
#if BELOWZERO
            || !SpriteManager.hasInitialized
#endif
                )
            {
                yield return null;
            }

            yield return new WaitForEndOfFrame();

            QuickLogger.Info($"Started patching. Version {version}");
            try
            {
                WorkingFileParser.HandleWorkingFiles();
                QuickLogger.Info($"Patching complete. Version {version}");
            }
            catch
            {
                QuickLogger.Error($"Critical error during file patching");
                throw;
            }

            HelpFilesWriter.HandleHelpFiles();
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
}
#endif