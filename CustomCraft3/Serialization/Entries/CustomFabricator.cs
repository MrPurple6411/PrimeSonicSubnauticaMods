#if !UNITY_EDITOR && (SUBNAUTICA || BELOWZERO)
namespace CustomCraft3.Serialization.Entries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using CustomCraft3.Interfaces;
    using CustomCraft3.Interfaces.InternalUse;
    using CustomCraft3.Serialization.Lists;
    using EasyMarkup;
    using Nautilus.Assets.PrefabTemplates;
    using Nautilus.Assets;
    using Nautilus.Crafting;
    using Nautilus.Handlers;
    using Nautilus.Utility;
    using UnityEngine;
    using IOPath = System.IO.Path;
    using static Nautilus.Assets.PrefabTemplates.FabricatorTemplate;

    internal partial class CustomFabricator : AliasRecipe, ICustomFabricator<CfCustomCraftingTab, CfMovedRecipe, CfAddedRecipe, CfAliasRecipe, CfCustomFood>, IFabricatorEntries
    {

        public Model Model
        {
            get => (Model)Enum.Parse(typeof(Model), model.Value, true);
            set => model.Value = value.ToString();
        }
        public override string[] TutorialText => CustomFabricatorTutorial;

        internal static readonly string[] CustomFabricatorTutorial = new[]
        {
        $"{CustomFabricatorList.ListKey}: Create your own fabricator with your own completely custom crafting tree!",
        $"    Custom fabricators have all the same properties as {AliasRecipeList.ListKey} with the following additions.",
        $"    {ModelKey}: Choose from one of three visual styles for your fabricator.",
        $"        Valid options are: {Model.Fabricator}|+{Model.MoonPool}|{Model.Workbench}",
        $"        This property is optional. Defaults to {Model.Fabricator}.",
        $"    {ColorTintKey}: This optional property lets you apply a color tint over your fabricator.",
        $"        This value is a list of floating point numbers.",
        $"        Use three numbers to set the value as RGB. Example: 1.0, 0.64, 0.0 makes an orange color.",
        $"        Use four numbers to set the value as RGBA (RGB with Alpha).",
        $"    {AllowedInBaseKey}: Determines if your fabricator can or can't be built inside a stationary base. ",
        $"        This property is optional. Defaults to YES.",
        $"    {AllowedInCyclopsKey}: Determines if your fabricator can or can't be built inside a Cyclops. ",
        $"        This property is optional. Defaults to YES.",
        $"    Everything to be added to the custom fabricator's crafting tree must be specified as a list inside the custom fabricator entry.",
        $"    Every entry will still need to specify a full path that includes the new fabricator as the starting point for the path.",
        $"    The lists you can add are as follows.",
        $"        {CfCustomCraftingTabListKey}: List of crafting tabs to be added to the custom fabricator.",
        $"        {CfAddedRecipeListKey}: List of added recipes for the custom fabricator.",
        $"        {CfAliasRecipeListKey}: List of alias recipes for the custom fabricator.",
        $"        {CfMovedRecipeListKey}: List of moved recipes for the custom fabricator.",
        $"        {CfCustomFoodListKey}: List of custom foods for the custom fabricator.",
    };

        public ModCraftTreeRoot RootNode { get; private set; }

        public override bool PassesPreValidation(OriginFile originFile)
        {
            PassedPreValidation = ItemIDisUnique() & InnerItemsAreValid() & FunctionalItemIsValid() & ValidFabricatorValues();

            if (PassedPreValidation)
                ValidateInternalEntries();

            return PassedPreValidation;
        }

        private bool ValidFabricatorValues()
        {
            switch (this.Model)
            {
                case Model.Fabricator:
                case Model.Workbench:
#if SUBNAUTICA
                case Model.MoonPool:
#endif
                    break;
                default:

                    QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} contained an invalue {ModelKey} value. Entry will be removed. Accepted values are only: {Model.Fabricator}|{Model.Workbench}"
#if SUBNAUTICA
                        + $"|{Model.MoonPool}"
#endif
                    );
                    return false;
            }

            if (!this.AllowedInBase && this.AllowedInCyclops)
            {

                QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} is denied from being built anywhere as both {AllowedInBaseKey} and {AllowedInCyclopsKey} are set to NO. Entry will be removed.");
                return false;
            }

            return true;
        }

        private bool ValidateInternalEntries()
        {
            StartCustomCraftingTree();
            ValidateEntries(this.CustomCraftingTabs, this.UniqueCustomTabs);
            ValidateEntries(this.AddedRecipes, this.UniqueAddedRecipes);
            ValidateEntries(this.AliasRecipes, this.UniqueAliasRecipes);
            ValidateEntries(this.MovedRecipes, this.UniqueMovedRecipes);
            ValidateEntries(this.CustomFoods, this.UniqueCustomFoods);

            // Secondary validation for entried that failed due to being incorrecltly ordered in the files.
            ValidateEntries(this.CustomCraftingTabs, this.UniqueCustomTabs);
            ValidateEntries(this.AddedRecipes, this.UniqueAddedRecipes);
            ValidateEntries(this.AliasRecipes, this.UniqueAliasRecipes);
            ValidateEntries(this.MovedRecipes, this.UniqueMovedRecipes);
            ValidateEntries(this.CustomFoods, this.UniqueCustomFoods);

            return true;
        }

        protected override bool FunctionalItemIsValid()
        {
            if (!string.IsNullOrEmpty(this.FunctionalID))
            {
                this.FunctionalID = string.Empty;

                QuickLogger.Warning($"{FunctionalIdKey} is not valid for {this.Key} entries. Was detected on '{this.ItemID}' from {this.Origin}. Please remove and try again.");
                return false;
            }

            return true;
        }

        internal void StartCustomCraftingTree()
        {
            if (this.TreeTypeID != CraftTree.Type.None)
                return;

            if (EnumHandler.TryAddEntry<CraftTree.Type>(ItemID, out var builder))
            {
                this.TreeTypeID = builder;
                builder.CreateCraftTreeRoot(out var root);
                this.RootNode = root;
            }
        }

        internal void FinishCustomCraftingTree()
        {
            SendToNautilus(this.UniqueCustomTabs);
            SendToNautilus(this.UniqueAddedRecipes);
            SendToNautilus(this.UniqueAliasRecipes);
            SendToNautilus(this.UniqueMovedRecipes);
            SendToNautilus(this.UniqueCustomFoods);
        }

        internal void HandleCraftTreeAddition<CraftingNode>(CraftingNode entry)
            where CraftingNode : ICustomFabricatorEntry, ITechTyped
        {
            try
            {
                CraftTreePath craftingNodePath = entry.GetCraftTreePath();
                if (craftingNodePath.HasError)
                {
                    QuickLogger.Error($"Encountered error in path for '{this.ItemID}' - Entry from {this.Origin} - Error Message: {craftingNodePath.Error}");
                    return;
                }

                QuickLogger.Debug($"Sending {entry.Key} '{entry.ItemID}' to be added to the crafting tree at '{craftingNodePath.RawPath}'");
                if (entry.IsAtRoot)
                {
                    this.RootNode.AddCraftingNode(entry.TechType);
                }
                else
                {
                    ModCraftTreeTab otherTab = this.RootNode.GetTabNode(craftingNodePath.StepsToParentTab);
                    otherTab.AddCraftingNode(entry.TechType);
                }
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {entry.Key} '{entry.ItemID}' from {this.Origin}", ex);

            }
        }

        internal void ValidateEntries<CustomCraftEntry>(EmPropertyCollectionList<CustomCraftEntry> collectionList, IDictionary<string, CustomCraftEntry> uniqueEntries)
            where CustomCraftEntry : EmPropertyCollection, ICustomCraft, ICustomFabricatorEntry, new()
        {
            var originalCount = collectionList.Count;
            var successCount = 0;
            var removals = new List<CustomCraftEntry>();
            foreach (CustomCraftEntry entry in collectionList)
            {
                entry.ParentFabricator = this;
                entry.Origin = this.Origin;
                if (!entry.PassesPreValidation(entry.Origin))
                    continue;

                if (!uniqueEntries.ContainsKey(entry.ID))
                {
                    // All checks passed
                    uniqueEntries.Add(entry.ID, entry);
                    removals.Add(entry);
                    successCount++;
                    continue;
                }

                if (entry is CfMovedRecipe movedRecipe && uniqueEntries[entry.ID] is CfMovedRecipe existingMovedRecipe)
                {
                    existingMovedRecipe.AlternatePaths.Add(movedRecipe.Origin, movedRecipe);
                    continue;
                }
                QuickLogger.Warning($"Duplicate entry for {entry.Key} '{entry.ID}' from {this.Origin} was already added by another working file. Kept first one. Discarded duplicate.");
            }

            foreach (CustomCraftEntry entry in removals)
                collectionList.Remove(entry);

            if (originalCount > 0)
                QuickLogger.Info($"{successCount} of {originalCount} {this.Key}:{typeof(CustomCraftEntry).Name} entries for {this.Key} staged for patching");
        }

        internal void SendToNautilus<CustomCraftEntry>(IDictionary<string, CustomCraftEntry> uniqueEntries)
            where CustomCraftEntry : ICustomCraft
        {
            int successCount = 0;
            foreach (CustomCraftEntry item in uniqueEntries.Values)
            {
                if (item.SendToNautilus())
                    successCount++;
            }

            if (uniqueEntries.Count > 0)
                QuickLogger.Info($"{successCount} of {uniqueEntries.Count} {typeof(CustomCraftEntry).Name} entries were patched");
        }

        protected override void HandleCustomPrefab()
        {
            if (this.TechType == TechType.None)
                throw new InvalidOperationException("TechTypeHandler.AddTechType must be called before PrefabHandler.RegisterPrefab.");

            CustomPrefab = new CustomPrefab(Info);

            FabricatorTemplate prefab = new FabricatorTemplate(Info, TreeTypeID)
            {
                FabricatorModel = Model,
                ColorTint = ColorTint,
            };

            prefab.ModifyPrefab += (obj) =>
            {

                Constructable constructible = obj.GetComponent<Constructable>();
                constructible.allowedInBase = AllowedInBase;
                constructible.allowedInSub = AllowedInCyclops;
                constructible.allowedOutside = false;
                constructible.allowedOnCeiling = false;
                constructible.allowedOnGround = Model == Model.Workbench;
                constructible.allowedOnWall = Model != Model.Workbench;
                constructible.allowedOnConstructables = false;
                constructible.controlModelState = true;
                constructible.rotationEnabled = false;
            };

            CustomPrefab.SetGameObject(prefab);
            CustomPrefab.Register();

            FinishCustomCraftingTree();
        }

        protected override void HandleCustomSprite()
        {
            Sprite sprite;

            string imagePath = IOPath.Combine(FileLocations.AssetsFolder, $"{this.ItemID}.png");
            if (File.Exists(imagePath))
            {
                QuickLogger.Debug($"Custom sprite found in Assets folder for {this.Key} '{this.ItemID}' from {this.Origin}");
                sprite = ImageUtils.LoadSpriteFromFile(imagePath);
            }
            else
            {
                QuickLogger.Debug($"Default sprite for {this.Key} '{this.ItemID}' from {this.Origin}");
                switch (this.Model)
                {
                    case Model.Fabricator:
                        sprite = SpriteManager.Get(TechType.Fabricator);
                        break;
                    case Model.Workbench:
                        sprite = SpriteManager.Get(TechType.Workbench);
                        break;
#if SUBNAUTICA
                    case Model.MoonPool:
                        imagePath = IOPath.Combine(FileLocations.AssetsFolder, $"MoonPool.png");
                        sprite = ImageUtils.LoadSpriteFromFile(imagePath);
                        break;
#endif
                    default:
                        throw new InvalidOperationException("Invalid ModelType encountered in HandleCustomSprite");
                }

            }

            SpriteHandler.RegisterSprite(this.TechType, sprite);
        }

        protected override void HandleCraftTreeAddition()
        {
            return; // Buildables aren't part of a crafting tree
        }

        public void DuplicateCustomTabDiscovered(string id)
        {
            QuickLogger.Warning($"Duplicate entry for {CustomCraftingTabList.ListKey} '{id}' from {this.Origin} was already added by another working file. Kept first one. Discarded duplicate.");
            if (this.UniqueCustomTabs.TryGetValue(id, out CfCustomCraftingTab tab))
            {
                tab.PassedSecondValidation = false;
                this.PassedSecondValidation = false;
                this.UniqueCustomTabs.Remove(id);
            }
        }

        public void DuplicateMovedRecipeDiscovered(string id)
        {
            QuickLogger.Warning($"Duplicate entry for {MovedRecipeList.ListKey} '{id}' from {this.Origin} was already added by another working file. Kept first one. Discarded duplicate.");
            if (this.UniqueMovedRecipes.TryGetValue(id, out CfMovedRecipe moved))
            {
                moved.PassedSecondValidation = false;
                this.PassedSecondValidation = false;
                this.UniqueMovedRecipes.Remove(id);
            }
        }

        public void DuplicateAddedRecipeDiscovered(string id)
        {
            QuickLogger.Warning($"Duplicate entry for {AddedRecipeList.ListKey} '{id}' from {this.Origin} was already added by another working file. Kept first one. Discarded duplicate.");
            if (this.UniqueAddedRecipes.TryGetValue(id, out CfAddedRecipe added))
            {
                added.PassedSecondValidation = false;
                this.PassedSecondValidation = false;
                this.UniqueAddedRecipes.Remove(id);
            }
        }

        public void DuplicateAliasRecipesDiscovered(string id)
        {
            QuickLogger.Warning($"Duplicate entry for {AliasRecipeList.ListKey} '{id}' from {this.Origin} was already added by another working file. Kept first one. Discarded duplicate.");
            if (this.UniqueAliasRecipes.TryGetValue(id, out CfAliasRecipe alias))
            {
                alias.PassedSecondValidation = false;
                this.PassedSecondValidation = false;
                this.UniqueAliasRecipes.Remove(id);
            }
        }

        public void DuplicateCustomFoodsDiscovered(string id)
        {
            QuickLogger.Warning($"Duplicate entry for {CustomFoodList.ListKey} '{id}' from {this.Origin} was already added by another working file. Kept first one. Discarded duplicate.");
            this.UniqueCustomFoods.Remove(id);
            if (this.UniqueCustomFoods.TryGetValue(id, out CfCustomFood food))
            {
                food.PassedSecondValidation = false;
                this.PassedSecondValidation = false;
                this.UniqueCustomFoods.Remove(id);
            }
        }
    }
}
#endif