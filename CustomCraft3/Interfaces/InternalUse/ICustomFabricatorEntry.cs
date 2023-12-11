namespace CustomCraft3.Interfaces.InternalUse
{
    using CustomCraft3.Serialization;
    using CustomCraft3.Serialization.Entries;

    internal interface ICustomFabricatorEntry : ICustomCraft
    {
        CustomFabricator ParentFabricator { get; set; }
        CraftTree.Type TreeTypeID { get; }
        bool IsAtRoot { get; }

#if !UNITY_EDITOR
        CraftTreePath GetCraftTreePath();
#endif
    }
}