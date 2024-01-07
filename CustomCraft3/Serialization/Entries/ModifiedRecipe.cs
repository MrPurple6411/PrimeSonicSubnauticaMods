#if !UNITY_EDITOR && (SUBNAUTICA || BELOWZERO)
namespace CustomCraft3.Serialization.Entries
{
    using System;
    using Common;
    using CustomCraft3.Interfaces;
    using CustomCraft3.Interfaces.InternalUse;
    using CustomCraft3.Serialization.Components;
    using Nautilus.Crafting;
    using Nautilus.Handlers;
    using static CraftData;

    internal partial class ModifiedRecipe : EmTechTyped, IModifiedRecipe, ICustomCraft
    {

        internal ModifiedRecipe(TechType origTechType) : this()
        {
            RecipeData origRecipe = CraftDataHandler.GetRecipeData(origTechType) ?? new RecipeData() { craftAmount = 0 };
            this.ItemID = origTechType.ToString();
            this.AmountCrafted = (short)origRecipe.craftAmount;

            for (int i = 0; i < origRecipe.ingredientCount; i++)
            {
                Ingredient origIngredient = origRecipe.GetIngredient(i);
                this.EmIngredients.Add(new EmIngredient(origIngredient.techType, (short)origIngredient.amount));
            }

            for (int i = 0; i < origRecipe.linkedItemCount; i++)
                linkedItems.Add(origRecipe.GetLinkedItem(i).AsString());
        }

        public override bool PassesPreValidation(OriginFile originFile)
        {
            return base.PassesPreValidation(originFile) & InnerItemsAreValid();
        }

        protected bool InnerItemsAreValid()
        {
            // Sanity check of the blueprints ingredients and linked items to be sure that it only contains known items
            // Modded items are okay, but they must be for mods the player already has installed
            return ValidateIngredients() &
                   ValidateLinkedItems() &
                   ValidateUnlocks() &
                   ValidateUnlockedBy();
        }

        protected bool ValidateUnlockedBy()
        {
            bool unlockedByValid = true;

            foreach (string unlockedBy in this.UnlockedBy)
            {
                TechType unlockByItemID = GetTechType(unlockedBy);

                if (unlockByItemID == TechType.None)
                {
                    if (!PassedPreValidation)
                        QuickLogger.Warning($"{this.Key} entry with ID of '{this.ItemID}' contained an unknown {this.UnlockedBy} '{unlockedBy}'. Entry will be discarded.");
                    unlockedByValid = false;
                    continue;
                }

                if (!UnlockedByItems.Contains(unlockByItemID))
                    this.UnlockedByItems.Add(unlockByItemID);
            }

            return unlockedByValid;
        }

        protected bool ValidateUnlocks()
        {
            bool unlocksValid = true;

            foreach (string unlockingItem in this.Unlocks)
            {
                TechType unlockingItemID = GetTechType(unlockingItem);

                if (unlockingItemID == TechType.None)
                {
                    if (!PassedPreValidation)
                        QuickLogger.Warning($"{this.Key} entry with ID of '{this.ItemID}' contained an unknown {UnlocksKey} '{unlockingItem}'. Entry will be discarded.");
                    unlocksValid = false;
                    continue;
                }

                if (!UnlockingItems.Contains(unlockingItemID))
                    this.UnlockingItems.Add(unlockingItemID);
            }

            return unlocksValid;
        }

        protected bool ValidateLinkedItems()
        {
            bool linkedItemsValid = true;
            
            LinkedItems.Clear();
            foreach (string linkedItem in this.LinkedItemIDs)
            {
                TechType linkedItemID = GetTechType(linkedItem);

                if (linkedItemID == TechType.None)
                {
                    if (!PassedPreValidation)
                        QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} contained an unknown {LinkedItemsIdsKey} '{linkedItem}'. Entry will be discarded.");
                    linkedItemsValid = false;
                    continue;
                }

                this.LinkedItems.Add(linkedItemID);
            }

            return linkedItemsValid;
        }

        protected bool ValidateIngredients()
        {
            bool ingredientsValid = true;

            foreach (EmIngredient ingredient in this.EmIngredients)
            {
                if (ingredient.PassesPreValidation(this.Origin))
                    this.Ingredients.Add(ingredient.ToIngredient());
                else
                    ingredientsValid = false;
            }

            return ingredientsValid;
        }

        public virtual bool SendToNautilus()
        {
            try
            {
                return
                    HandleModifiedRecipe() &&
                    HandleUnlocks();
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {this.Key} entry '{this.ItemID}' from {this.Origin}", ex);
                return false;
            }
        }

        protected bool HandleModifiedRecipe()
        {
            RecipeData original = CraftDataHandler.GetRecipeData(this.TechType);

            if (original == null)
            {
                QuickLogger.Warning($"Original recipe for '{this.ItemID}' from {this.Origin} could not be found for modification. Entry will be discarded.");
                return false;  // Unknown recipe
            }

            var replacement = new RecipeData();

            bool overrideRecipe = false;
            string changes = "";
            // Amount
            if (this.AmountCrafted.HasValue)
            {
                overrideRecipe |= true;
                changes += $" {AmountCraftedKey} ";
                replacement.craftAmount = this.AmountCrafted.Value;
            }
            else
            {
                replacement.craftAmount = original.craftAmount;
            }

            // Ingredients
            if (this.EmIngredients.Count > 0)
            {
                overrideRecipe |= true;
                changes += $" {IngredientsKey} ";
                replacement.Ingredients = this.Ingredients;
            }
            else
            {
                // Copy original ingredients
                for (int i = 0; i < original.ingredientCount; i++)
                {
                    replacement.Ingredients.Add(
                        new Ingredient(
                        original.GetIngredient(i).techType,
                        original.GetIngredient(i).amount));
                }
            }

            // Linked Items
            if (this.LinkedItems.Count > 0)
            {
                overrideRecipe |= true;
                changes += $" {LinkedItemsIdsKey}";
                replacement.LinkedItems = this.LinkedItems;
            }
            else
            {
                // Copy original linked items
                for (int i = 0; i < original.linkedItemCount; i++)
                    replacement.LinkedItems.Add(original.GetLinkedItem(i));
            }

            if (overrideRecipe)
            {
                CraftDataHandler.SetRecipeData(this.TechType, replacement);
                QuickLogger.Debug($"Modifying recipe for '{this.ItemID}' from {this.Origin} with new values in: {changes}");
            }

            return true;
        }

        protected bool HandleUnlocks()
        {
            if (this.ForceUnlockAtStart)
            {
                KnownTechHandler.UnlockOnStart(this.TechType);
                QuickLogger.Debug($"{this.Key} for '{this.ItemID}' from {this.Origin} will be a unlocked at the start of the game");
            }

            if (this.UnlockingItems.Count > 0)
            {
                KnownTechHandler.SetAnalysisTechEntry(this.TechType, this.UnlockingItems);
            }

            if (this.UnlockedByItems.Count > 0)
            {
                TechType[] thisTechType = new[] { this.TechType };
                foreach (TechType unlockedByItem in this.UnlockedByItems)
                {
                    KnownTechHandler.SetAnalysisTechEntry(unlockedByItem, thisTechType);
                }
            }

            return true;
        }
    }
}
#endif