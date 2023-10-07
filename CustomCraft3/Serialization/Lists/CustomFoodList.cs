namespace CustomCraft3.Serialization.Lists;

using CustomCraft3.Serialization.Entries;
using EasyMarkup;

internal class CustomFoodList : EmPropertyCollectionList<CustomFood>
{
    internal const string ListKey = "CustomFoods";

    public CustomFoodList() : base(ListKey)
    {
    }
}
