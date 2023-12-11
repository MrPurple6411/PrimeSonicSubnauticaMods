namespace CustomCraft3.Serialization.Lists
{
    using CustomCraft3.Serialization.Entries;
    using EasyMarkup;

    internal class ModifiedRecipeList : EmPropertyCollectionList<ModifiedRecipe>
    {
        internal const string ListKey = "ModifiedRecipes";

        public ModifiedRecipeList() : base(ListKey)
        {
        }
    }
}