namespace CustomCraft3.Serialization.Entries;

using System.Collections.Generic;
using CustomCraft3.Interfaces.InternalUse;
using CustomCraft3.Serialization;
using EasyMarkup;

internal class CfMovedRecipe : MovedRecipe, ICustomFabricatorEntry
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

    public CraftTreePath GetCraftTreePath()
    {
        return new CraftTreePath(this.NewPath, this.ItemID);
    }

    protected override void HandleCraftTreeAddition()
    {
        this.ParentFabricator.HandleCraftTreeAddition(this);
    }

    internal override EmProperty Copy()
    {
        return new CfMovedRecipe(this.Key, this.CopyDefinitions);
    }
}
