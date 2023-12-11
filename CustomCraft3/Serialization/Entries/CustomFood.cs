#if !UNITY_EDITOR
namespace CustomCraft3.Serialization.Entries
{
    using System;
    using System.IO;
    using Common;
    using CustomCraft3.Interfaces;
    using CustomCraft3.Interfaces.InternalUse;
    using Nautilus.Handlers;
    using Nautilus.Utility;
    using IOPath = System.IO.Path;
    using Nautilus.Assets.PrefabTemplates;
    using Nautilus.Assets;

    internal partial class CustomFood : AliasRecipe, ICustomFood, ICustomCraft
    {
        public override bool PassesPreValidation(OriginFile originFile)
        {
            return ValidateCustomFoodValues() & base.PassesPreValidation(originFile);
        }

        private bool ValidateCustomFoodValues()
        {
            if (this.FoodValue < MinValue || this.FoodValue > MaxValue)
            {
                if (PassedPreValidation)
                    QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} has {FoodKey} values out of range. Must be between {MinValue} and {MaxValue}. Entry will be discarded.");
                return false;
            }

            if (this.WaterValue < MinValue || this.FoodValue > MaxValue)
            {
                if (PassedPreValidation)
                    QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} has {WaterKey} values out of range. Must be between {MinValue} and {MaxValue}. Entry will be discarded.");
                return false;
            }

            if (this.WaterValue == 0 & this.FoodValue == 0)
            {
                if (PassedPreValidation)
                    QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' must have at least one non-zero value for either {FoodKey} or {WaterKey}. Entry will be discarded.");
                return false;
            }

            return true;
        }

        protected override void HandleCustomSprite()
        {
            string imagePath = IOPath.Combine(FileLocations.AssetsFolder, $"{this.ItemID}.png");

            if (File.Exists(imagePath))
            {
                QuickLogger.Debug($"Custom sprite found in Assets folder for the custom sprite of {this.Key} '{this.ItemID}' from {this.Origin}");
                SpriteHandler.RegisterSprite(this.TechType, ImageUtils.LoadSpriteFromFile(imagePath));
                return;
            }

            if (this.SpriteItemID > TechType.None && this.SpriteItemID < TechType.Databox)
            {
                QuickLogger.Debug($"{SpriteItemIdKey} '{this.SpriteItemID}' used for the custom sprite of {this.Key} '{this.ItemID}' from {this.Origin}");
                SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get(this.SpriteItemID));
                return;
            }

            if (this.FoodType != FoodModel.None)
            {
                QuickLogger.Debug($"{FoodModelKey} '{this.FoodType}' used for the custom sprite of {this.Key} '{this.ItemID}' from {this.Origin}");
                SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get((TechType)this.FoodType));
                return;
            }

            // More defaulting behavior
            imagePath = IOPath.Combine(FileLocations.AssetsFolder, this.DefaultIconFileName);

            if (File.Exists(imagePath))
            {
                QuickLogger.Debug($"Default sprite used for {this.Key} '{this.ItemID}' from {this.Origin}");
                SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get((TechType)this.FoodType));
                return;
            }

            QuickLogger.Warning($"Missing all custom sprites for  {this.Key} '{this.ItemID}' from {this.Origin}. Using last fallback.");
            TechType fallbackIcon = this.FoodValue >= this.WaterValue
                ? TechType.NutrientBlock
                : TechType.FilteredWater;

            SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get(fallbackIcon));
        }

        protected override void HandleCustomPrefab()
        {
            if (this.TechType == TechType.None)
                throw new InvalidOperationException("TechTypeHandler.AddTechType must be called before PrefabHandler.RegisterPrefab.");

            Info = new PrefabInfo(ItemID, $"{ItemID}Prefab", TechType);
            CustomPrefab = new CustomPrefab(Info);

            var foodPrefab = new CloneTemplate(Info, FoodPrefab);
            foodPrefab.ModifyPrefab += (obj) =>
            {
                Eatable eatable = obj.EnsureComponent<Eatable>();

                eatable.foodValue = FoodValue;
                eatable.waterValue = WaterValue;
                eatable.decomposes = Decomposes;
                eatable.kDecayRate = DecayRateMod * 0.015f;
                obj.SetActive(false);
            };

            CustomPrefab.SetGameObject(foodPrefab);
            CustomPrefab.Register();

#if SUBNAUTICA
            if (this.UseDrinkSound)
            {
                CraftDataHandler.SetEatingSound(this.TechType, "event:/player/drink");
            }
#endif
        }
    }
}
#endif