namespace CustomCraft3.Serialization.Entries
{
    using System.Collections.Generic;
    using EasyMarkup;

    internal partial class CfMovedRecipe : MovedRecipe
    {
        public CfMovedRecipe() : this(TypeName, MovedRecipeProperties)
        {
        }

        protected CfMovedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
        }

        public CustomFabricator ParentFabricator { get; set; }

        public CraftTree.Type TreeTypeID => this.ParentFabricator.TreeTypeID;

        public bool IsAtRoot => this.NewPath == this.ParentFabricator.ItemID;

        public override EmProperty Copy()
        {
            return new CfMovedRecipe(this.Key, this.CopyDefinitions);
        }
    }
}