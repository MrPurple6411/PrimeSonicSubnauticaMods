namespace CustomCraft3.Interfaces
{
    using System.Collections.Generic;
    using CustomCraft3.Serialization.Components;

    internal interface IModifiedRecipe : ITechTyped
    {
        short? AmountCrafted { get; }
        bool ForceUnlockAtStart { get; }

        IList<EmIngredient> EmIngredients { get; }
        IList<string> LinkedItemIDs { get; }
        IList<string> Unlocks { get; }
        IList<string> UnlockedBy { get; }
    }
}