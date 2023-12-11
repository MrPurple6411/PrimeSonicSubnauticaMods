namespace CustomCraft3.Serialization.Entries
{
    using System.Collections.Generic;
    using CustomCraft3.Serialization.Lists;
    using EasyMarkup;

    internal partial class AddedRecipe : ModifiedRecipe
    {
        protected const string PathKey = "Path";
        protected const string PdaGroupKey = "PdaGroup";
        protected const string PdaCategoryKey = "PdaCategory";

        public const string TypeName = "AddedRecipe";

        public override string[] TutorialText => AddedRecipeTutorial;

        internal static readonly string[] AddedRecipeTutorial = new[]
        {
       $"{AddedRecipeList.ListKey}: Adding your own recipes into any of the existing fabricators.",
       $"    {AddedRecipeList.ListKey} have all the same properties as {ModifiedRecipeList.ListKey}, with the following additions:",
       $"    {PathKey}: Sets the fabricator and crafting tab where the new recipe will be added.",
        "        Remember, this must be a valid path to an existing tab or to a custom tab you've created.",
        "        You can find a full list of all original crafting paths for all the standard fabricators in the OriginalRecipes folder.",
       $"    {PdaGroupKey}: Sets the main group for blueprint shown in the PDA.",
       $"        This is optional. If {PdaGroupKey} is set, {PdaCategoryKey} must also be set.",
       $"    {PdaCategoryKey}: Sets the category under the group for blueprint shown in the PDA.",
       $"        This is optional. If {PdaCategoryKey} is set, {PdaGroupKey} must also be set.",
    };

        protected readonly EmProperty<string> path;
        protected readonly EmProperty<TechGroup> techGroup;
        protected readonly EmProperty<TechCategory> techCategory;

        public string Path
        {
            get => path.Value;
            set => path.Value = value;
        }

        protected static List<EmProperty> AddedRecipeProperties => new List<EmProperty>(ModifiedRecipeProperties)
    {
        new EmProperty<string>(PathKey),
        new EmProperty<TechGroup>(PdaGroupKey, TechGroup.Uncategorized) { Optional = true },
        new EmProperty<TechCategory>(PdaCategoryKey, TechCategory.Misc) { Optional = true }
    };

        public TechGroup PdaGroup
        {
            get => techGroup.Value;
            set => techGroup.Value = value;
        }

        public TechCategory PdaCategory
        {
            get => techCategory.Value;
            set => techCategory.Value = value;
        }

        public AddedRecipe() : this(TypeName, AddedRecipeProperties)
        {
        }

        protected AddedRecipe(string key) : this(key, AddedRecipeProperties)
        {
        }

        protected AddedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            path = (EmProperty<string>)Properties[PathKey];
            techGroup = (EmProperty<TechGroup>)Properties[PdaGroupKey];
            techCategory = (EmProperty<TechCategory>)Properties[PdaCategoryKey];
            DefaultForceUnlock = true;
        }
        public override EmProperty Copy()
        {
            return new AddedRecipe(this.Key, this.CopyDefinitions);
        }

    }
}