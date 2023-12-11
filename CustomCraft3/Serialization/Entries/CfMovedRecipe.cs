#if !UNITY_EDITOR
namespace CustomCraft3.Serialization.Entries
{
    using CustomCraft3.Interfaces.InternalUse;
    using CustomCraft3.Serialization;

    internal partial class CfMovedRecipe : MovedRecipe, ICustomFabricatorEntry
    {
        public CraftTreePath GetCraftTreePath()
        {
            return new CraftTreePath(this.NewPath, this.ItemID);
        }

        protected override void HandleCraftTreeAddition()
        {
            this.ParentFabricator.HandleCraftTreeAddition(this);
        }
    }
}
#endif