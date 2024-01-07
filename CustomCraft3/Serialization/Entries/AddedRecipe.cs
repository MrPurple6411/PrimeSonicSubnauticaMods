#if !UNITY_EDITOR && (SUBNAUTICA || BELOWZERO)
namespace CustomCraft3.Serialization.Entries
{
    using System;
    using Common;
    using CustomCraft3.Interfaces;
    using CustomCraft3.Serialization;
    using CustomCraft3.Serialization.Components;
    using Nautilus.Crafting;
    using Nautilus.Handlers;
    using static CraftData;

    internal partial class AddedRecipe : ModifiedRecipe, IAddedRecipe
    {
        public override bool SendToNautilus()
        {
            try
            {
                HandleAddedRecipe();

                HandleCraftTreeAddition();

                HandleUnlocks();

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling Added Recipe '{this.ItemID}' from  from {this.Origin}", ex);
                return false;
            }
        }

        protected void HandleAddedRecipe(short defaultCraftAmount = 1)
        {
            if (CraftDataHandler.GetRecipeData(this.TechType) == null)
                QuickLogger.Debug($"Adding new recipe for '{this.ItemID}' with recipe from {this.Origin}");
            else
                QuickLogger.Debug($"Replacing recipe for '{this.ItemID}' with recipe from {this.Origin}");

            RecipeData replacement = CreateRecipeTechData(defaultCraftAmount);
            CraftDataHandler.SetRecipeData(this.TechType, replacement);

            if (this.PdaGroup != TechGroup.Uncategorized)
            {
                CraftDataHandler.AddToGroup(this.PdaGroup, this.PdaCategory, this.TechType);
                // Nautilus logs enough here
            }
        }

        internal RecipeData CreateRecipeTechData(short defaultCraftAmount = 1)
        {
            var replacement = new RecipeData
            {
                craftAmount = this.AmountCrafted ?? defaultCraftAmount
            };

            foreach (EmIngredient ingredient in this.EmIngredients)
                replacement.Ingredients.Add(new Ingredient(ingredient.TechType, ingredient.Required));

            foreach (TechType linkedItem in this.LinkedItems)
                replacement.LinkedItems.Add(linkedItem);

            return replacement;
        }

        protected virtual void HandleCraftTreeAddition()
        {
            var craftPath = new CraftTreePath(this.Path, this.ItemID);

            if (craftPath.HasError)
                QuickLogger.Error($"Encountered error in path for '{this.ItemID}' - Entry from {this.Origin} - Error Message: {craftPath.Error}");
            else
                AddCraftNode(craftPath, this.TechType, this.Origin);
        }

    }
}
#endif