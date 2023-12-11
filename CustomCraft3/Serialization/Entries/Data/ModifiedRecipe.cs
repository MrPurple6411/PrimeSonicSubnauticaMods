namespace CustomCraft3.Serialization.Entries
{
    using System.Collections.Generic;
    using CustomCraft3.Interfaces.InternalUse;
    using CustomCraft3.Serialization.Components;
    using CustomCraft3.Serialization.Lists;
    using EasyMarkup;
    using static CraftData;

    internal partial class ModifiedRecipe : EmTechTyped, ICustomCraft
    {
        public virtual string[] TutorialText => ModifiedRecipeTutorial;

        internal static readonly string[] ModifiedRecipeTutorial = new[]
        {
       $"{ModifiedRecipeList.ListKey}: Modify an existing crafting recipe. ",
       $"    {IngredientsKey}: Completely replace a recipe's required ingredients.",
        "        This is optional if you don't want to change the required ingredients.",
       $"    {AmountCraftedKey}: Change how many copies of the item are created when you craft the recipe.",
        "        This is optional if you don't want to change how many copies of the item are created at once.",
       $"    {LinkedItemsIdsKey}: Add or modify the linked items that are created when the recipe is crafted.",
        "        This is optional if you don't want to change what items are crafted with this one.",
       $"    {UnlocksKey}: Set other recipes to be unlocked when you analyze or craft this one.",
        "        This is optional if you don't want to change what gets unlocked when you scan or craft this item.",
       $"    {ForceUnlockAtStartKey}: You can also set if this recipe should be unlocked at the start or not. Make sure you have a recipe unlocking this one.",
        "        This is optional. For Added Recipes, this defaults to 'YES'.",
        $"    {UnlockedByKey}: Set this recipe to be unlocked when any one of the items listed here gets unlocked.",
        "        This is optional. If this is for an existing recipe, note that the original unlocks will not be affected.",
    };

        protected const string AmountCraftedKey = "AmountCrafted";
        protected const string IngredientsKey = "Ingredients";
        protected const string LinkedItemsIdsKey = "LinkedItemIDs";
        protected const string ForceUnlockAtStartKey = "ForceUnlockAtStart";
        protected const string UnlocksKey = "Unlocks";
        protected const string UnlockedByKey = "UnlockedBy";

        protected readonly EmProperty<short> amountCrafted;
        protected readonly EmPropertyCollectionList<EmIngredient> ingredients;
        protected readonly EmPropertyList<string> linkedItems;
        protected readonly EmYesNo unlockedAtStart;
        protected readonly EmPropertyList<string> unlocks;
        protected readonly EmPropertyList<string> unlockedBy;

        public string ID => this.ItemID;

        public short? AmountCrafted
        {
            get
            {
                if (amountCrafted.HasValue && amountCrafted.Value > -1)
                    return amountCrafted.Value;

                return null;
            }
            set => amountCrafted.Value = value.Value;
        }

        protected bool DefaultForceUnlock = false;

        public bool ForceUnlockAtStart
        {
            get
            {
                if (unlockedAtStart.HasValue)
                    return unlockedAtStart.Value;

                return DefaultForceUnlock;
            }

            set => unlockedAtStart.Value = value;
        }

        public IList<string> Unlocks => unlocks.Values;
        protected List<TechType> UnlockingItems { get; } = new List<TechType>();

        public IList<string> UnlockedBy => unlockedBy.Values;
        protected List<TechType> UnlockedByItems { get; } = new List<TechType>();

        public IList<EmIngredient> EmIngredients => ingredients.Values;
        protected List<Ingredient> Ingredients { get; } = new List<Ingredient>();

        public IList<string> LinkedItemIDs => linkedItems.Values;
        protected List<TechType> LinkedItems { get; } = new List<TechType>();

        protected static List<EmProperty> ModifiedRecipeProperties => new List<EmProperty>(TechTypedProperties)
    {
        new EmProperty<short>(AmountCraftedKey, 1) { Optional = true },
        new EmPropertyCollectionList<EmIngredient>(IngredientsKey) { Optional = true },
        new EmPropertyList<string>(LinkedItemsIdsKey) { Optional = true },
        new EmYesNo(ForceUnlockAtStartKey) { Optional = true },
        new EmPropertyList<string>(UnlocksKey) { Optional = true },
        new EmPropertyList<string>(UnlockedByKey) { Optional = true },
    };

        public OriginFile Origin { get; set; }

        public bool PassedSecondValidation { get; internal set; } = true;

        public ModifiedRecipe() : this("ModifiedRecipe", ModifiedRecipeProperties)
        {
        }

        protected ModifiedRecipe(string key) : this(key, ModifiedRecipeProperties)
        {
        }

        protected ModifiedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            amountCrafted = (EmProperty<short>)Properties[AmountCraftedKey];
            ingredients = (EmPropertyCollectionList<EmIngredient>)Properties[IngredientsKey];
            linkedItems = (EmPropertyList<string>)Properties[LinkedItemsIdsKey];
            unlockedAtStart = (EmYesNo)Properties[ForceUnlockAtStartKey];
            unlocks = (EmPropertyList<string>)Properties[UnlocksKey];
            unlockedBy = (EmPropertyList<string>)Properties[UnlockedByKey];

            OnValueExtractedEvent += ValueExtracted;
        }

        private void ValueExtracted()
        {
            foreach (EmIngredient ingredient in ingredients)
            {
                string itemID = (ingredient["ItemID"] as EmProperty<string>).Value;
                short required = (ingredient["Required"] as EmProperty<short>).Value;
            }
        }

        public override EmProperty Copy()
        {
            return new ModifiedRecipe(this.Key, this.CopyDefinitions);
        }
    }
}