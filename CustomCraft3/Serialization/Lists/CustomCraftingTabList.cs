namespace CustomCraft3.Serialization.Lists
{
    using CustomCraft3.Serialization.Entries;
    using EasyMarkup;

    internal class CustomCraftingTabList : EmPropertyCollectionList<CustomCraftingTab>
    {
        internal const string ListKey = "CustomCraftingTabs";

        public CustomCraftingTabList() : base(ListKey)
        {
        }
    }
}