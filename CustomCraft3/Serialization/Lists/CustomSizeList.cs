namespace CustomCraft3.Serialization.Lists
{
    using CustomCraft3.Serialization.Entries;
    using EasyMarkup;

    internal class CustomSizeList : EmPropertyCollectionList<CustomSize>
    {
        internal const string ListKey = "CustomSizes";

        public CustomSizeList() : base(ListKey)
        {
        }
    }
}