namespace CustomCraft3.Serialization.Lists;

using CustomCraft3.Serialization.Entries;
using EasyMarkup;

internal class AliasRecipeList : EmPropertyCollectionList<AliasRecipe>
{
    internal const string ListKey = "AliasRecipes";

    public AliasRecipeList() : base(ListKey)
    {
    }
}
