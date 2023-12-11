namespace CustomCraft3.Serialization.Entries
{
    using System.Collections.Generic;
    using EasyMarkup;

    internal partial class CfCustomCraftingTab : CustomCraftingTab
    {
        public CfCustomCraftingTab() : this(TypeName, CustomCraftingTabProperties)
        {
            base.OnValueExtractedEvent -= ParsePath;
        }

        protected CfCustomCraftingTab(string key, ICollection<EmProperty> definitions) : base()
        {

        }

        public CustomFabricator ParentFabricator { get; set; }

        public CraftTree.Type TreeTypeID => this.ParentFabricator.TreeTypeID;

        public bool IsAtRoot => this.ParentTabPath == this.ParentFabricator.ItemID;

        public override EmProperty Copy()
        {
            return new CfCustomCraftingTab(this.Key, this.CopyDefinitions);
        }

    }
}