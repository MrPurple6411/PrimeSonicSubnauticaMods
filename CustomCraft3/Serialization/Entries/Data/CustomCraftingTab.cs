namespace CustomCraft3.Serialization.Entries
{
    using System.Collections.Generic;
    using CustomCraft3.Interfaces.InternalUse;
    using CustomCraft3.Serialization;
    using CustomCraft3.Serialization.Lists;
    using EasyMarkup;

    internal partial class CustomCraftingTab : EmPropertyCollection, ICustomCraft
    {
        public string[] TutorialText => CustomCraftingTabTutorial;

        internal static readonly string[] CustomCraftingTabTutorial = new[]
        {
       $"{CustomCraftingTabList.ListKey}: Add your own custom tabs into the fabricator crafting trees. ",
        "    An absolute must for organizing large numbers of crafts.",
       $"    {TabIdKey}: This uniquely identifies the tab.",
       $"        If you want to use a custom sprite for your tab, the file must be named exactly as the {TabIdKey}",
       $"        This option will take priority over the {SpriteItemIdKey}.",
       $"    {DisplayNameKey}: The tab name you will see in-game.",
       $"    {SpriteItemIdKey}: Alternative way to set the tab sprite, by re-using the sprite of an existing in-game item.",
       $"        This option will be used only if a png file matching the {TabIdKey} isn't found in the Assets folder.",
       $"    {ParentTabPathKey}: This defines where your tab begins on the crafting tree.",
        "        You can have as many custom tabs as you want, and even include custom tabs inside other custom tabs.",
        "        Just make sure you add your custom tabs to the file in the correct order, from inside to outside.",
        "        If a custom tab goes inside another custom tab, then the parent tab must be placed above the child tab.",
        "        You can find a full list of all original crafting paths for all the standard fabricators in the OriginalRecipes folder.",
    };

        public const string TypeName = "CustomTab";
        protected const string TabIdKey = "TabID";
        protected const string DisplayNameKey = "DisplayName";
        protected const string SpriteItemIdKey = "SpriteItemID";
        protected const string ParentTabPathKey = "ParentTabPath";

        protected readonly EmProperty<string> emTabID;
        protected readonly EmProperty<string> emDisplayName;
        protected readonly EmProperty<TechType> emSpriteID;
        protected readonly EmProperty<string> emParentTabPath;

        public CraftTreePath CraftingPath { get; protected set; }

        protected static ICollection<EmProperty> CustomCraftingTabProperties => new List<EmProperty>(4)
    {
        new EmProperty<string>(TabIdKey),
        new EmProperty<string>(DisplayNameKey),
        new EmProperty<TechType>(SpriteItemIdKey) { Optional = true },
        new EmProperty<string>(ParentTabPathKey),
    };

        public CustomCraftingTab() : this(TypeName, CustomCraftingTabProperties)
        {
        }

        protected CustomCraftingTab(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emTabID = (EmProperty<string>)Properties[TabIdKey];
            emDisplayName = (EmProperty<string>)Properties[DisplayNameKey];
            emSpriteID = (EmProperty<TechType>)Properties[SpriteItemIdKey];
            emParentTabPath = (EmProperty<string>)Properties[ParentTabPathKey];

            base.OnValueExtractedEvent += ParsePath;
        }

        public OriginFile Origin { get; set; }

        protected void ParsePath()
        {
            try
            {
                this.CraftingPath = new CraftTreePath(this.ParentTabPath, this.TabID);
            }
            catch
            {
                this.CraftingPath = null;
            }
        }

        public string ID => this.TabID;

        public string TabID
        {
            get => emTabID.Value;
            set => emTabID.Value = value;
        }

        public string DisplayName
        {
            get => emDisplayName.Value;
            set => emDisplayName.Value = value;
        }

        public TechType SpriteItemID
        {
            get => emSpriteID.Value;
            set => emSpriteID.Value = value;
        }

        public string ParentTabPath
        {
            get => emParentTabPath.Value;
            set => emParentTabPath.Value = value;
        }

        public string FullPath => $"{this.ParentTabPath}{"/"}{this.TabID}";

        public bool PassedPreValidation { get; set; } = true;
        public bool PassedSecondValidation { get; set; } = true;

        public override EmProperty Copy()
        {
            return new CustomCraftingTab(this.Key, this.CopyDefinitions);
        }
    }
}