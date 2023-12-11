namespace CustomCraft3.Serialization.Entries
{
    using System.Collections.Generic;
    using CustomCraft3.Interfaces.InternalUse;
    using CustomCraft3.Serialization;
    using CustomCraft3.Serialization.Components;
    using CustomCraft3.Serialization.Lists;
    using EasyMarkup;

    internal partial class MovedRecipe : EmTechTyped, ICustomCraft
    {
        private const string OldPathKey = "OldPath";
        private const string NewPathKey = "NewPath";
        private const string HiddenKey = "Hidden";
        private const string CopyKey = "Copied";

        public const string TypeName = "MovedRecipe";

        public string[] TutorialText => MovedRecipeTutorial;

        internal static readonly string[] MovedRecipeTutorial = new[]
        {
       $"{MovedRecipeList.ListKey}: Further customize the crafting tree to your liking.",
       $"    All moved recipes work with existing crafting nodes. So all moved recipe entries should be for items that can already be crafted.",
       $"    {OldPathKey}: If you want to move a craft node from its original location, this must be set.",
       $"    {OldPathKey}: If you want to move a craft node from its original location, this must be set.",
       $"        This node is optional if {CopyKey} is set to 'YES'.",
       $"        This node must be present if {HiddenKey} is set to 'YES'.",
       $"        This cannot be used to access paths in modded or custom fabricators.",
       $"    {NewPathKey}: If you want to move or copy the recipe to a new location, set the path here. It could even be a different (non-custom) crafting tree.",
       $"        This node is optional if {HiddenKey} is set to 'YES'.",
       $"        This node must be present if {CopyKey} is set to 'YES'.",
       $"        if neither {CopyKey} or {HiddenKey} are present, then the recipe will be removed from the {OldPathKey} and added to the {NewPathKey}.",
       $"    {CopyKey}: If you want, you can copy the recipe to the new path without removing the original by setting this to 'YES'.",
       $"        This node is optional and will default to 'NO' when not present.",
       $"        {CopyKey} cannot be set to 'YES' if {HiddenKey} is also set to 'YES'.",
       $"        Moved recipes will be handled after all other crafts, so if you added a new recipe, you can copy it to more than one fabricator.",
       $"    {HiddenKey}: Or you can set this property to 'YES' to simply remove the crafting node instead.",
       $"        This node is optional and will default to 'NO' when not present.",
       $"        {HiddenKey} cannot be set to 'YES' if {CopyKey} is also set to 'YES'.",
        "    Remember, all paths must be valid and point to existing tab or to a custom tab you've created.",
        "    You can find a full list of all original crafting paths for all the standard fabricators in the OriginalRecipes folder.",
    };

        private readonly EmProperty<string> oldPath;
        private readonly EmProperty<string> newPath;
        private readonly Dictionary<OriginFile, MovedRecipe> alternatePaths = new Dictionary<OriginFile, MovedRecipe>();
        private readonly EmYesNo hidden;
        private readonly EmYesNo copied;

        protected static List<EmProperty> MovedRecipeProperties => new List<EmProperty>(TechTypedProperties)
    {
        new EmProperty<string>(OldPathKey){ Optional = true },
        new EmProperty<string>(NewPathKey){ Optional = true },
        new EmYesNo(HiddenKey, false){ Optional = true },
        new EmYesNo(CopyKey, false){ Optional = true }
    };

        public OriginFile Origin { get; set; }

        public bool PassedSecondValidation { get; set; } = true;

        public MovedRecipe() : this(TypeName, MovedRecipeProperties)
        {
        }

        protected MovedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            oldPath = (EmProperty<string>)Properties[OldPathKey];
            newPath = (EmProperty<string>)Properties[NewPathKey];
            hidden = (EmYesNo)Properties[HiddenKey];
            copied = (EmYesNo)Properties[CopyKey];
        }

        public string OldPath
        {
            get => oldPath.Value;
            set => oldPath.Value = value;
        }

        public string NewPath
        {
            get => newPath.Value;
            set => newPath.Value = value;
        }

        public Dictionary<OriginFile, MovedRecipe> AlternatePaths => alternatePaths;

        public bool Hidden
        {
            get => hidden.Value;
            set => hidden.Value = value;
        }

        public bool Copied
        {
            get => copied.Value;
            set => copied.Value = value;
        }

        public string ID => this.ItemID;

        internal static List<MovedRecipe> MovedRecipesWithErrors = new List<MovedRecipe>();

        internal Dictionary<CraftTreePath, OriginFile> FailedPaths = new Dictionary<CraftTreePath, OriginFile>();

        public override EmProperty Copy()
        {
            return new MovedRecipe(this.Key, this.CopyDefinitions);
        }
    }
}