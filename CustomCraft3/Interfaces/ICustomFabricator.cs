namespace CustomCraft3.Interfaces;

using CustomCraft3.Serialization.Entries;
using EasyMarkup;
using UnityEngine;
using static Nautilus.Assets.PrefabTemplates.FabricatorTemplate;

internal interface ICustomFabricator<Tab, Moved, Added, Alias, Food> : IAliasRecipe
    where Tab : EmPropertyCollection, ICraftingTab, new()
    where Moved : EmPropertyCollection, IMovedRecipe, new()
    where Added : EmPropertyCollection, IAddedRecipe, new()
    where Alias : EmPropertyCollection, IAliasRecipe, new()
    where Food : EmPropertyCollection, ICustomFood, new()
{
    Model Model { get; }
    Color ColorTint { get; }
    bool AllowedInBase { get; }
    bool AllowedInCyclops { get; }

    EmPropertyCollectionList<Tab> CustomCraftingTabs { get; }
    EmPropertyCollectionList<Moved> MovedRecipes { get; }
    EmPropertyCollectionList<Added> AddedRecipes { get; }
    EmPropertyCollectionList<Alias> AliasRecipes { get; }
    EmPropertyCollectionList<Food> CustomFoods { get; }
}
