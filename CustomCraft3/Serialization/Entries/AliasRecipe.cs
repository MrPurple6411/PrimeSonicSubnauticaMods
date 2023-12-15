#if !UNITY_EDITOR
namespace CustomCraft3.Serialization.Entries
{
    using System;
    using System.IO;
    using Common;
    using CustomCraft3.Interfaces;
    using Nautilus.Assets;
    using Nautilus.Assets.PrefabTemplates;
    using Nautilus.Handlers;
    using Nautilus.Utility;
    using IOPath = System.IO.Path;
#if SUBNAUTICA
    using Sprite = Atlas.Sprite;
#elif BELOWZERO
    using Sprite = UnityEngine.Sprite;
#endif

    internal partial class AliasRecipe: AddedRecipe, IAliasRecipe
    {

        public PrefabInfo Info { get; protected set; }
        public CustomPrefab CustomPrefab { get; protected set; }

        public override bool PassesPreValidation(OriginFile originFile)
        {
            PassedPreValidation = ItemIDisUnique() & // Confirm that the item ID is unique.
                                  InnerItemsAreValid() & // Confirm that all inner items are valid.
                                  FunctionalItemIsValid(); // Confirm that the functional item is valid.

            return PassedPreValidation;
        }

        protected bool ItemIDisUnique()
        {
            if (this.TechType != TechType.None)
            {
                // If the item ID is already a valid TechType, then it's already been registered.
                return true;
            }

            if (TechTypeExtensions.FromString(this.ItemID, out _, true))
            {
                if (!PassedPreValidation)
                    QuickLogger.Warning($"Duplicate {ItemIdKey} value '{this.ItemID}' found in {this.Key} entry from {this.Origin}.");
                return false;
            }

            if (EnumHandler.TryGetValue<TechType>(this.ItemID, out _))
            {
                if (!PassedPreValidation)
                    QuickLogger.Warning($"Duplicate {ItemIdKey} value '{this.ItemID}' found in {this.Key} entry from {this.Origin}.");
                return false;
            }

            // Alias Recipes must request their techtype be added during the validation step
            if (!EnumHandler.TryAddEntry<TechType>(this.ItemID, out var builder))
            {
                if (!PassedPreValidation)
                    QuickLogger.Warning($"Unable to create new TechType with {ItemIdKey} value '{this.ItemID}' for entry {this.Key} from {this.Origin} is specifies an {ItemIdKey}.");
                return false;
            }

            this.TechType = builder;
            builder.WithPdaInfo(this.DisplayName, this.Tooltip, "English", this.ForceUnlockAtStart);
            Info = new PrefabInfo(this.ItemID, this.ItemID + "Prefab", this.TechType);

            return true;
        }

        protected virtual bool FunctionalItemIsValid()
        {
            if (string.IsNullOrEmpty(this.FunctionalID))
                return true; // No value provided. This is fine.

            // The functional item for cloning must be valid.
            this.FunctionalCloneID = GetTechType(this.FunctionalID);

            if (this.FunctionalCloneID == TechType.None)
            {
                if (!PassedPreValidation)
                    QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} contained an unknown {FunctionalIdKey} value '{this.FunctionalID}'. Entry will be discarded.");
                return false;
            }

            return true;
        }

        public override bool SendToNautilus()
        {
            try
            {
                //  See if there is an asset in the asset folder that has the same name
                HandleCustomSprite();

                // Alias recipes should default to not producing the custom item unless explicitly configured
                HandleAddedRecipe(0);

                HandleCraftTreeAddition();

                HandleUnlocks();

                HandleCustomPrefab();

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {this.Key} '{this.ItemID}' from {this.Origin}", ex);
                return false;
            }
        }

        protected virtual void HandleCustomSprite()
        {
            string imagePath = IOPath.Combine(FileLocations.AssetsFolder, $"{this.ItemID}.png");

            if (File.Exists(imagePath))
            {
                QuickLogger.Debug($"Custom sprite found in Assets folder for {this.Key} '{this.ItemID}' from {this.Origin}");
                Sprite sprite = ImageUtils.LoadSpriteFromFile(imagePath);
                SpriteHandler.RegisterSprite(this.TechType, sprite);
                return;
            }

            if (this.SpriteItemID > TechType.None && this.SpriteItemID < TechType.Databox)
            {
                QuickLogger.Debug($"{SpriteItemIdKey} '{this.SpriteItemID}' used for {this.Key} '{this.ItemID}' from {this.Origin}");
                Sprite sprite = SpriteManager.Get(this.SpriteItemID);
                SpriteHandler.RegisterSprite(this.TechType, sprite);
                return;
            }

            if (this.LinkedItems.Count > 0)
            {
                QuickLogger.Debug($"First entry in {LinkedItemsIdsKey} used for icon of {this.Key} '{this.ItemID}' from {this.Origin}");
                Sprite sprite = SpriteManager.Get(this.LinkedItems[0]);
                SpriteHandler.RegisterSprite(this.TechType, sprite);
                return;
            }

            QuickLogger.Warning($"No sprite loaded for {this.Key} '{this.ItemID}' from {this.Origin}");
        }

        protected virtual void HandleCustomPrefab()
        {
            if (this.FunctionalCloneID != TechType.None)
            {
                CustomPrefab = new CustomPrefab(Info);
                var foodPrefab = new CloneTemplate(Info, FunctionalCloneID) { ModifyPrefab = (go) => go.SetActive(false) };
                CustomPrefab.SetGameObject(foodPrefab);
                CustomPrefab.Register();

                QuickLogger.Debug($"Custom item '{this.ItemID}' will be a functional clone of '{this.FunctionalID}' - Entry from {this.Origin}");
            }
        }
    }
}
#endif