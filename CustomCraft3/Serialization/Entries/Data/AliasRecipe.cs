namespace CustomCraft3.Serialization.Entries
{
    using System.Collections.Generic;
    using CustomCraft3.Serialization.Lists;
    using EasyMarkup;

    internal partial class AliasRecipe : AddedRecipe
    {
        protected const string DisplayNameKey = "DisplayName";
        protected const string ToolTipKey = "Tooltip";
        protected const string FunctionalIdKey = "FunctionalID";
        protected const string SpriteItemIdKey = "SpriteItemID";

        public new const string TypeName = "AliasRecipe";

        public override string[] TutorialText => AliasRecipeTutorial;

        internal static readonly string[] AliasRecipeTutorial = new[]
        {
       $"{AliasRecipeList.ListKey}: A powerful tool with multiple applications.",
        "    Alias recipes allow you to create multiple ways to craft the same item, bypassing one of the limitations of Subnautica's crafting system.",
        "    Alias recipes can also be used to add your own custom items into the game, all without any coding, with some limitations.",
       $"    Alias recipes should NOT include an {AmountCraftedKey} value. {LinkedItemsIdsKey} should be used instead to define the produce being crafted.",
       $"    {AliasRecipeList.ListKey} have all the same properties of {AddedRecipeList.ListKey}, but when creating your own items, you will want to include these new properties:",
       $"        {DisplayNameKey}: Sets the in-game name for the new item.",
       $"        {ToolTipKey}: Sets the in-game tooltip text whenever you hover over the item.",
       $"        {FunctionalIdKey}: Choose an existing item in the game and clone that item's in-game functions into your custom item.",
        "            Without this property, any user created item will be non-functional in-game, usable as a crafting component but otherwise useful for nothing else.",
       $"        {SpriteItemIdKey}: Use the in-game sprite of an existing item for your custom item.",
       $"            This option will be used only if a png file matching the {ItemIdKey} isn't found in the Assets folder.",
        "            If no file is found with that name, the sprite for the first LinkedItem will be used instead.",
        "            This should only be used with non-modded item values.",
    };

        protected readonly EmProperty<string> displayName;
        protected readonly EmProperty<string> tooltip;
        protected readonly EmProperty<string> functionalID;
        protected readonly EmProperty<TechType> spriteID;

        public string DisplayName
        {
            get => displayName.Value;
            set => displayName.Value = value;
        }

        public string Tooltip
        {
            get => tooltip.Value;
            set => tooltip.Value = value;
        }

        public string FunctionalID
        {
            get => functionalID.Value;
            set => functionalID.Value = value;
        }

        public TechType SpriteItemID
        {
            get => spriteID.Value;
            set => spriteID.Value = value;
        }

        public TechType FunctionalCloneID { get; private set; }

        protected static List<EmProperty> AliasRecipeProperties => new List<EmProperty>(AddedRecipeProperties)
    {
        new EmProperty<string>(DisplayNameKey),
        new EmProperty<string>(ToolTipKey),
        new EmProperty<string>(FunctionalIdKey) { Optional = true },
        new EmProperty<TechType>(SpriteItemIdKey, TechType.None) { Optional = true }
    };

        public AliasRecipe() : this(TypeName, AliasRecipeProperties)
        {
        }

        protected AliasRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            displayName = (EmProperty<string>)Properties[DisplayNameKey];
            tooltip = (EmProperty<string>)Properties[ToolTipKey];
            functionalID = (EmProperty<string>)Properties[FunctionalIdKey];
            spriteID = (EmProperty<TechType>)Properties[SpriteItemIdKey];

            amountCrafted.DefaultValue = 0;
        }

        public override EmProperty Copy()
        {
            return new AliasRecipe(this.Key, this.CopyDefinitions);
        }

    }
}