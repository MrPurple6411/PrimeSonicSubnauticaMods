#if !UNITY_EDITOR
namespace CustomCraft3.Serialization.Entries
{
    using System;
    using System.IO;
    using Common;
    using CustomCraft3.Interfaces;
    using CustomCraft3.Interfaces.InternalUse;
    using CustomCraft3.Serialization;
    using EasyMarkup;
    using Nautilus.Handlers;
    using Nautilus.Utility;
    using IOPath = System.IO.Path;
#if SUBNAUTICA
    using Sprite = Atlas.Sprite;
#endif

    internal partial class CustomCraftingTab : EmPropertyCollection, ICraftingTab, ICustomCraft
    {
        public bool PassesPreValidation(OriginFile originFile)
        {
            return (PassedPreValidation = this.CraftingPath != null & ValidFabricator());
        }

        protected virtual bool ValidFabricator()
        {
            if (this.CraftingPath.Scheme == CraftTree.Type.None)
            {
                QuickLogger.Error($"Error on crafting tab '{this.TabID}'. {ParentTabPathKey} must identify a fabricator for the custom tab.");
                return false;
            }

            return true;
        }

        public virtual bool SendToNautilus()
        {
            try
            {
                HandleCraftingTab();

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {this.Key} '{this.TabID}' from {this.Origin}", ex);
                return false;
            }
        }

        protected void HandleCraftingTab()
        {
            if (this.CraftingPath.HasError)
            {
                QuickLogger.Error($"Encountered error in path for '{this.TabID}' - Entry from {this.Origin} - Error Message: {this.CraftingPath.Error}");
                return;
            }

            Sprite sprite = GetCraftingTabSprite();

            if (this.CraftingPath.IsAtRoot)
            {
                CraftTreeHandler.AddTabNode(this.CraftingPath.Scheme, this.TabID, this.DisplayName, sprite);
            }
            else
            {
                CraftTreeHandler.AddTabNode(this.CraftingPath.Scheme, this.TabID, this.DisplayName, sprite, this.CraftingPath.StepsToParentTab);
            }
        }

        protected Sprite GetCraftingTabSprite()
        {
            string imagePath = IOPath.Combine(FileLocations.AssetsFolder, this.TabID + @".png");

            if (File.Exists(imagePath))
            {
                QuickLogger.Debug($"Custom sprite found in Assets folder for {this.Key} '{this.TabID}' from {this.Origin}");
                return ImageUtils.LoadSpriteFromFile(imagePath);
            }

            if (this.SpriteItemID != TechType.None)
            {
                QuickLogger.Debug($"SpriteItemID used for {this.Key} '{this.TabID}' from {this.Origin}");
                return SpriteManager.Get(this.SpriteItemID);
            }

            QuickLogger.Warning($"No sprite loaded for {this.Key} '{this.TabID}' from {this.Origin}");
            return SpriteManager.Get(TechType.None);
        }
    }
}
#endif