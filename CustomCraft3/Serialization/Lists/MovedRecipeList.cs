namespace CustomCraft3.Serialization.Lists;

using CustomCraft3.Serialization.Entries;
using EasyMarkup;

internal class MovedRecipeList : EmPropertyCollectionList<MovedRecipe>
{
    internal const string ListKey = "MovedRecipes";

    public MovedRecipeList() : base(ListKey)
    {
    }
}
