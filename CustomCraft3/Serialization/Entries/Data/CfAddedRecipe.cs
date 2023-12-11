namespace CustomCraft3.Serialization.Entries
{
    using System.Collections.Generic;
    using EasyMarkup;

    internal partial class CfAddedRecipe : AddedRecipe
    {
        public CfAddedRecipe() : this(TypeName, AddedRecipeProperties)
        {
        }

        protected CfAddedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
        }

        public CustomFabricator ParentFabricator { get; set; }

        public CraftTree.Type TreeTypeID => this.ParentFabricator.TreeTypeID;

        public bool IsAtRoot => this.Path == this.ParentFabricator.ItemID;

        public override EmProperty Copy()
        {
            return new CfAddedRecipe(this.Key, this.CopyDefinitions);
        }

    }
}