namespace CustomCraft3.Serialization.Lists
{
    using CustomCraft3.Serialization.Entries;
    using EasyMarkup;

    internal class CustomFragmentCountList : EmPropertyCollectionList<CustomFragmentCount>
    {
        internal const string ListKey = "CustomFragmentCounts";

        public CustomFragmentCountList() : base(ListKey)
        {
        }
    }
}