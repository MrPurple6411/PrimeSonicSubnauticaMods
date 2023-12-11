namespace CustomCraft3.Serialization.Entries
{
    using System.Collections.Generic;
    using CustomCraft3.Serialization.Components;
    using CustomCraft3.Serialization.Lists;
    using EasyMarkup;
    using UnityEngine;

    internal partial class CustomFabricator : AliasRecipe
    {
        protected const string ModelKey = "Model";
        protected const string ColorTintKey = "ColorTint";
        protected const string AllowedInBaseKey = "AllowedInBase";
        protected const string AllowedInCyclopsKey = "AllowedInCyclops";
        protected const string CfCustomCraftingTabListKey = CustomCraftingTabList.ListKey;
        protected const string CfAliasRecipeListKey = AliasRecipeList.ListKey;
        protected const string CfAddedRecipeListKey = AddedRecipeList.ListKey;
        protected const string CfMovedRecipeListKey = MovedRecipeList.ListKey;
        protected const string CfCustomFoodListKey = CustomFoodList.ListKey;

        protected readonly EmProperty<string> model;
        protected readonly EmColorRGB colortint;
        protected readonly EmYesNo allowedInBase;
        protected readonly EmYesNo allowedInCyclops;

        protected static List<EmProperty> CustomFabricatorProperties => new List<EmProperty>(AliasRecipeProperties)
    {
        new EmProperty<string>(ModelKey, "Fabricator"),
        new EmColorRGB(ColorTintKey) { Optional = true },
        new EmYesNo(AllowedInBaseKey, true) { Optional = true },
        new EmYesNo(AllowedInCyclopsKey, true) { Optional = true },
        new EmPropertyCollectionList<CfCustomCraftingTab>(CfCustomCraftingTabListKey) { Optional = true },
        new EmPropertyCollectionList<CfMovedRecipe>(CfMovedRecipeListKey) { Optional = true },
        new EmPropertyCollectionList<CfAddedRecipe>(CfAddedRecipeListKey) { Optional = true },
        new EmPropertyCollectionList<CfAliasRecipe>(CfAliasRecipeListKey) { Optional = true },
        new EmPropertyCollectionList<CfCustomFood>(CfCustomFoodListKey) { Optional = true },
    };

        public CustomFabricator() : this("CustomFabricator", CustomFabricatorProperties)
        {
        }

        protected CustomFabricator(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            model = (EmProperty<string>)Properties[ModelKey];
            colortint = (EmColorRGB)Properties[ColorTintKey];
            allowedInBase = (EmYesNo)Properties[AllowedInBaseKey];
            allowedInCyclops = (EmYesNo)Properties[AllowedInCyclopsKey];
            this.CustomCraftingTabs = (EmPropertyCollectionList<CfCustomCraftingTab>)Properties[CfCustomCraftingTabListKey];
            this.MovedRecipes = (EmPropertyCollectionList<CfMovedRecipe>)Properties[CfMovedRecipeListKey];
            this.AddedRecipes = (EmPropertyCollectionList<CfAddedRecipe>)Properties[CfAddedRecipeListKey];
            this.AliasRecipes = (EmPropertyCollectionList<CfAliasRecipe>)Properties[CfAliasRecipeListKey];
            this.CustomFoods = (EmPropertyCollectionList<CfCustomFood>)Properties[CfCustomFoodListKey];

            path.Optional = true;
        }

        public Color ColorTint => colortint.GetColor();

        internal bool HasColorValue => colortint.HasValue;

        public bool AllowedInBase
        {
            get => allowedInBase.Value;
            set => allowedInBase.Value = value;
        }

        public bool AllowedInCyclops
        {
            get => allowedInCyclops.Value;
            set => allowedInCyclops.Value = value;
        }

        // Set in constructor
        public EmPropertyCollectionList<CfCustomCraftingTab> CustomCraftingTabs { get; }
        public EmPropertyCollectionList<CfMovedRecipe> MovedRecipes { get; }
        public EmPropertyCollectionList<CfAddedRecipe> AddedRecipes { get; }
        public EmPropertyCollectionList<CfAliasRecipe> AliasRecipes { get; }
        public EmPropertyCollectionList<CfCustomFood> CustomFoods { get; }

        private IDictionary<string, CfCustomCraftingTab> UniqueCustomTabs { get; } = new Dictionary<string, CfCustomCraftingTab>();
        private IDictionary<string, CfMovedRecipe> UniqueMovedRecipes { get; } = new Dictionary<string, CfMovedRecipe>();
        private IDictionary<string, CfAddedRecipe> UniqueAddedRecipes { get; } = new Dictionary<string, CfAddedRecipe>();
        private IDictionary<string, CfAliasRecipe> UniqueAliasRecipes { get; } = new Dictionary<string, CfAliasRecipe>();
        private IDictionary<string, CfCustomFood> UniqueCustomFoods { get; } = new Dictionary<string, CfCustomFood>();

        public string ListKey { get; }

        public CraftTree.Type TreeTypeID { get; private set; }

        public ICollection<string> CustomTabIDs => this.UniqueCustomTabs.Keys;
        public ICollection<string> MovedRecipeIDs => this.UniqueMovedRecipes.Keys;
        public ICollection<string> AddedRecipeIDs => this.UniqueAddedRecipes.Keys;
        public ICollection<string> AliasRecipesIDs => this.UniqueAliasRecipes.Keys;
        public ICollection<string> CustomFoodIDs => this.UniqueCustomFoods.Keys;

        public override EmProperty Copy()
        {
            return new CustomFabricator(this.Key, this.CopyDefinitions);
        }
    }
}