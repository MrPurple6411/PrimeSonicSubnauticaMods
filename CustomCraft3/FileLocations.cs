﻿using System.IO;
using System.Reflection;
using CustomCraft3.Serialization;

namespace CustomCraft3
{
    internal static class FileLocations
    {
        internal const string RootModName = "CustomCraft3";
        internal const string ModFriendlyName = "Custom Craft 3";
        internal static string FolderRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        internal static string SamplesFolder = Path.Combine(FolderRoot, "SampleFiles");
        internal static string OriginalsFolder = Path.Combine(FolderRoot, "OriginalRecipes");
        internal static string HowToFile = Path.Combine(FolderRoot, "README_HowToUseThisMod.txt");
        internal static string WorkingFolder = Path.Combine(FolderRoot, "WorkingFiles");
        internal static string AssetsFolder = Path.Combine(FolderRoot, "Assets");
        internal static string ModJson = Path.Combine(FolderRoot, "mod.json");
        internal static string ConfigFile = Path.Combine(FolderRoot, CustomCraft2Config.FileName);
    }
}