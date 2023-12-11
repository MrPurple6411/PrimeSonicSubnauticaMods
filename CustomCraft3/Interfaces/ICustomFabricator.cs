namespace CustomCraft3.Interfaces
{
    using EasyMarkup;
    using UnityEngine;
#if !UNITY_EDITOR
    using static Nautilus.Assets.PrefabTemplates.FabricatorTemplate;
#endif

    internal interface ICustomFabricator<Tab, Moved, Added, Alias, Food> : IAliasRecipe
        where Tab : EmPropertyCollection, ICraftingTab, new()
        where Moved : EmPropertyCollection, IMovedRecipe, new()
        where Added : EmPropertyCollection, IAddedRecipe, new()
        where Alias : EmPropertyCollection, IAliasRecipe, new()
        where Food : EmPropertyCollection, ICustomFood, new()
    {
#if !UNITY_EDITOR
        Model Model { get; }
#endif
        Color ColorTint { get; }
        bool AllowedInBase { get; }
        bool AllowedInCyclops { get; }

        EmPropertyCollectionList<Tab> CustomCraftingTabs { get; }
        EmPropertyCollectionList<Moved> MovedRecipes { get; }
        EmPropertyCollectionList<Added> AddedRecipes { get; }
        EmPropertyCollectionList<Alias> AliasRecipes { get; }
        EmPropertyCollectionList<Food> CustomFoods { get; }
    }
}