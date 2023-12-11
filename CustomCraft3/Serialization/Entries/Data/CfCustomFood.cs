namespace CustomCraft3.Serialization.Entries
{
    using System.Collections.Generic;
    using EasyMarkup;

    internal partial class CfCustomFood : CustomFood
    {
        public CfCustomFood() : this(TypeName, CustomFoodProperties)
        {
        }

        protected CfCustomFood(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
        }

        public CustomFabricator ParentFabricator { get; set; }

        public CraftTree.Type TreeTypeID => this.ParentFabricator.TreeTypeID;

        public bool IsAtRoot => this.Path == this.ParentFabricator.ItemID;

        public override EmProperty Copy()
        {
            return new CfCustomFood(this.Key, this.CopyDefinitions);
        }
    }
}