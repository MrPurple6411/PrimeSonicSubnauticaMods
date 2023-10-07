﻿namespace CustomCraft3;

using CustomCraft3.Interfaces;
using Nautilus.Assets;
using UnityEngine;

internal class FunctionalClone : Spawnable
{
    internal readonly TechType BaseItem;
    public FunctionalClone(IAliasRecipe aliasRecipe, TechType baseItem)
        : base(aliasRecipe.ItemID, $"{aliasRecipe.ItemID}Prefab", aliasRecipe.Tooltip)
    {
        BaseItem = baseItem;
        this.TechType = aliasRecipe.TechType; // TechType already handled by this point
    }

    public override string AssetsFolder { get; } = FileLocations.RootModName + "/Assets";

    public override GameObject GetGameObject()
    {
        GameObject prefab = CraftData.GetPrefabForTechType(BaseItem);
        return GameObject.Instantiate(prefab);
    }
}
