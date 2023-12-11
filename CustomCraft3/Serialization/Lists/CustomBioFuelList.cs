namespace CustomCraft3.Serialization.Lists
{
    using CustomCraft3.Serialization.Entries;
    using EasyMarkup;

    internal class CustomBioFuelList : EmPropertyCollectionList<CustomBioFuel>
    {
        internal const string ListKey = "CustomBioFuels";

        public CustomBioFuelList() : base(ListKey)
        {
        }
    }
}