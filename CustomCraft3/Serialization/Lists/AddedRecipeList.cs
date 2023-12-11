namespace CustomCraft3.Serialization.Lists
{
    using CustomCraft3.Serialization.Entries;
    using EasyMarkup;

    internal class AddedRecipeList : EmPropertyCollectionList<AddedRecipe>
    {
        internal const string ListKey = "AddedRecipes";

        public AddedRecipeList() : base(ListKey)
        {
        }
    }
}