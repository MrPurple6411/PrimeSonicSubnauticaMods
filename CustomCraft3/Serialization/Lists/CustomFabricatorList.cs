namespace CustomCraft3.Serialization.Lists
{
    using CustomCraft3.Serialization.Entries;
    using EasyMarkup;

    internal class CustomFabricatorList : EmPropertyCollectionList<CustomFabricator>
    {
        internal const string ListKey = "CustomFabricators";

        public CustomFabricatorList() : base(ListKey)
        {
        }
    }
}