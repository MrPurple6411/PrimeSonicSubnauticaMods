﻿#if !UNITY_EDITOR && (SUBNAUTICA || BELOWZERO)
namespace CustomCraft3
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Common;
    using CustomCraft3.Interfaces.InternalUse;
    using CustomCraft3.Serialization;
    using CustomCraft3.Serialization.Entries;
    using CustomCraft3.Serialization.Lists;
    using EasyMarkup;

    internal static class WorkingFileParser
    {
        private static readonly ParsingPackage<CustomCraftingTab, CustomCraftingTabList> CustomTabs = new ParsingPackage<CustomCraftingTab, CustomCraftingTabList>(CustomCraftingTabList.ListKey);
        private static readonly ParsingPackage<MovedRecipe, MovedRecipeList> MovedRecipes = new ParsingPackage<MovedRecipe, MovedRecipeList>(MovedRecipeList.ListKey);
        private static readonly ParsingPackage<AddedRecipe, AddedRecipeList> AddedRecipes = new ParsingPackage<AddedRecipe, AddedRecipeList>(AddedRecipeList.ListKey);
        private static readonly ParsingPackage<AliasRecipe, AliasRecipeList> AliasRecipes = new ParsingPackage<AliasRecipe, AliasRecipeList>(AliasRecipeList.ListKey);
        private static readonly ParsingPackage<CustomFabricator, CustomFabricatorList> CustomFabricators = new ParsingPackage<CustomFabricator, CustomFabricatorList>(CustomFabricatorList.ListKey);
        private static readonly ParsingPackage<ModifiedRecipe, ModifiedRecipeList> ModifiedRecipes = new ParsingPackage<ModifiedRecipe, ModifiedRecipeList>(ModifiedRecipeList.ListKey);
        private static readonly ParsingPackage<CustomSize, CustomSizeList> CustomSizes = new ParsingPackage<CustomSize, CustomSizeList>(CustomSizeList.ListKey);
        private static readonly ParsingPackage<CustomBioFuel, CustomBioFuelList> CustomBioFuels = new ParsingPackage<CustomBioFuel, CustomBioFuelList>(CustomBioFuelList.ListKey);
        private static readonly ParsingPackage<CustomFragmentCount, CustomFragmentCountList> CustomFragCounts = new ParsingPackage<CustomFragmentCount, CustomFragmentCountList>(CustomFragmentCountList.ListKey);
        private static readonly ParsingPackage<CustomFood, CustomFoodList> CustomFoods = new ParsingPackage<CustomFood, CustomFoodList>(CustomFoodList.ListKey);

        internal static readonly IEnumerable<IParsingPackage> OrderedPackages = new List<IParsingPackage>(10)
    {
        CustomFabricators,
        CustomTabs,
        AddedRecipes,
        AliasRecipes,
        ModifiedRecipes,
        CustomFoods,
        MovedRecipes,
        CustomFragCounts,
        CustomSizes,
        CustomBioFuels,
    };

        private static readonly IDictionary<string, IParsingPackage> PackagesLookup = new Dictionary<string, IParsingPackage>(10);

        internal static void HandleWorkingFiles()
        {
            foreach (IParsingPackage package in OrderedPackages)
                PackagesLookup.Add(package.ListKey, package);

            int rollingCount = 0;
            // Handle loose files
            rollingCount += ParseAndPatchFiles(Directory.GetFiles(FileLocations.WorkingFolder), "WorkingFiles");

            // Handle sub-folders
            foreach (var workingDirectory in Directory.GetDirectories(FileLocations.WorkingFolder))
                rollingCount += ParseAndPatchFiles(Directory.GetFiles(workingDirectory), $"WorkingFiles/{Path.GetDirectoryName(workingDirectory)}");

            if (rollingCount == 0)
            {
                QuickLogger.Info($"No files with valid Entries found in {FileLocations.WorkingFolder}");
                return;
            }

            foreach (IParsingPackage package in OrderedPackages)
                package.PrePassValidation();

            foreach (IParsingPackage package in OrderedPackages)
                package.SecondPassValidation();

            MasterUniquenessValidation();

            QuickLogger.Debug($"Sending requests to Nautilus");
            foreach (IParsingPackage package in OrderedPackages)
                package.SendToNautilus();

            // Retry failed paths if any exist due to missing CraftTree that got added by another file.
            if (MovedRecipe.MovedRecipesWithErrors.Count > 0)
            {
                QuickLogger.Debug($"Retrying failed paths incase the CraftTree had not been made yet.");
                foreach (var movedRecipe in MovedRecipe.MovedRecipesWithErrors)
                {
                    movedRecipe.RetryFailedPaths();
                }
            }
        }

        private static int ParseAndPatchFiles(string[] workingFiles, string directory)
        {
            QuickLogger.Info($"{workingFiles.Length} files found in the {directory} folder");

            int rollingCount = 0;
            foreach (string file in workingFiles)
                rollingCount += DeserializeFile(file);

            QuickLogger.Info($"{rollingCount} entries successfully discovered across files in {directory}");
            return rollingCount;
        }

        private static int DeserializeFile(string workingFilePath)
        {
            string fileName = Path.GetFileName(workingFilePath);

            string serializedData = File.ReadAllText(workingFilePath, Encoding.UTF8);

            if (string.IsNullOrEmpty(serializedData))
            {
                QuickLogger.Warning($"File '{fileName}' contained no text");
                return 0;
            }

            if (EmProperty.CheckKey(serializedData, out string key))
            {
                int check;
                if (PackagesLookup.TryGetValue(key, out IParsingPackage package))
                {
                    check = package.ParseEntries(serializedData, OriginFile.GetOriginFile(fileName));
                }
                else
                {
                    QuickLogger.Warning($"Unknown primary key '{key}' detected in file '{fileName}'");
                    return 0;
                }

                switch (check)
                {
                    case -1:
                        QuickLogger.Warning($"Unable to parse file '{fileName}'");
                        break;
                    case 0:
                        QuickLogger.Warning($"File '{fileName}' was parsed but no entries were found");
                        break;
                    default:
                        QuickLogger.Info($"{check} entries parsed from file '{fileName}'");
                        return check;
                }
            }
            else
            {
                QuickLogger.Warning($"Could not identify primary key in file '{fileName}'");
            }

            return 0;
        }

        private static void MasterUniquenessValidation()
        {
            var allTabs = new HashSet<string>(CustomTabs.UniqueEntries.Keys, StringComparer.InvariantCultureIgnoreCase);
            var allMoves = new HashSet<string>(MovedRecipes.UniqueEntries.Keys, StringComparer.InvariantCultureIgnoreCase);
            var allAdded = new HashSet<string>(AddedRecipes.UniqueEntries.Keys, StringComparer.InvariantCultureIgnoreCase);
            var allAlias = new HashSet<string>(AliasRecipes.UniqueEntries.Keys, StringComparer.InvariantCultureIgnoreCase);
            var allFoods = new HashSet<string>(CustomFoods.UniqueEntries.Keys, StringComparer.InvariantCultureIgnoreCase);

            foreach (IFabricatorEntries entries in CustomFabricators.UniqueEntries.Values)
            {
                QuickLogger.Debug("Checking uniqueness of CustomFabricator TabIDs");
                ValidateSets(allTabs, entries.CustomTabIDs, entries.DuplicateCustomTabDiscovered);

                QuickLogger.Debug("Checking uniqueness of CustomFabricator MovedRecipeIDs");
                ValidateSets(allMoves, entries.MovedRecipeIDs, entries.DuplicateMovedRecipeDiscovered);

                QuickLogger.Debug("Checking uniqueness of CustomFabricator AddedRecipeIDs");
                ValidateSets(allAdded, entries.AddedRecipeIDs, entries.DuplicateAddedRecipeDiscovered);

                QuickLogger.Debug("Checking uniqueness of CustomFabricator AliasRecipesIDs");
                ValidateSets(allAlias, entries.AliasRecipesIDs, entries.DuplicateAliasRecipesDiscovered);

                QuickLogger.Debug("Checking uniqueness of CustomFabricator CustomFoodIDs");
                ValidateSets(allFoods, entries.CustomFoodIDs, entries.DuplicateCustomFoodsDiscovered);
            }
        }

        private static void ValidateSets(HashSet<string> masterSet, ICollection<string> setToCheck, Action<string> informDuplicate)
        {
            var dups = new List<string>();
            foreach (string id in setToCheck)
            {
                if (masterSet.Contains(id))
                    dups.Add(id);
                else
                    masterSet.Add(id);
            }

            foreach (string dupId in dups)
            {
                informDuplicate.Invoke(dupId);
            }
        }
    }
}
#endif