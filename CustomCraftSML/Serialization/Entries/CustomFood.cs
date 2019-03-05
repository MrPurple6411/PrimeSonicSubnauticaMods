﻿namespace CustomCraft2SML.Serialization.Entries
{
    using Common;
    using Common.EasyMarkup;
    using CustomCraft2SML.Interfaces;
    using CustomCraft2SML.Interfaces.InternalUse;
    using CustomCraft2SML.SMLHelperItems;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using IOPath = System.IO.Path;

    public enum FoodModel
    {
        None = TechType.None,
        BigFilteredWater = TechType.BigFilteredWater,
        DisinfectedWater = TechType.DisinfectedWater,
        FilteredWater = TechType.FilteredWater,
        StillsuitWater = TechType.StillsuitWater,
        BulboTreePiece = TechType.BulboTreePiece,
        PurpleVegetable = TechType.PurpleVegetable,
        CreepvinePiece = TechType.CreepvinePiece,
        JellyPlant = TechType.JellyPlant,
        KooshChunk = TechType.KooshChunk,
        HangingFruit = TechType.HangingFruit,
        Melon = TechType.Melon,
        NutrientBlock = TechType.NutrientBlock,
        CookedPeeper = TechType.CookedPeeper,
        CookedHoleFish = TechType.CookedHoleFish,
        CookedGarryFish = TechType.CookedGarryFish,
        CookedReginald = TechType.CookedReginald,
        CookedBladderfish = TechType.CookedBladderfish,
        CookedHoverfish = TechType.CookedHoverfish,
        CookedSpadefish = TechType.CookedSpadefish,
        CookedBoomerang = TechType.CookedBoomerang,
        CookedEyeye = TechType.CookedEyeye,
        CookedOculus = TechType.CookedOculus,
        CookedHoopfish = TechType.CookedHoopfish,
        CookedSpinefish = TechType.CookedSpinefish,
        CookedLavaEyeye = TechType.CookedLavaEyeye,
        CookedLavaBoomerang = TechType.CookedLavaBoomerang,
        CuredPeeper = TechType.CuredPeeper,
        CuredHoleFish = TechType.CuredHoleFish,
        CuredGarryFish = TechType.CuredGarryFish,
        CuredReginald = TechType.CuredReginald,
        CuredBladderfish = TechType.CuredBladderfish,
        CuredHoverfish = TechType.CuredHoverfish,
        CuredSpadefish = TechType.CuredSpadefish,
        CuredBoomerang = TechType.CuredBoomerang,
        CuredEyeye = TechType.CuredEyeye,
        CuredOculus = TechType.CuredOculus,
        CuredHoopfish = TechType.CuredHoopfish,
        CuredSpinefish = TechType.CuredSpinefish,
        CuredLavaEyeye = TechType.CuredLavaEyeye,
        CuredLavaBoomerang = TechType.CuredLavaBoomerang,
    }

    internal class CustomFood : AliasRecipe, ICustomFood, ICustomCraft
    {
        // We may need this later.
        internal static bool IsMappedFoodType(TechType techType)
        {
            switch ((int)techType)
            {
                case (int)FoodModel.None:
                case (int)FoodModel.BigFilteredWater:
                case (int)FoodModel.DisinfectedWater:
                case (int)FoodModel.FilteredWater:
                case (int)FoodModel.StillsuitWater:
                case (int)FoodModel.BulboTreePiece:
                case (int)FoodModel.PurpleVegetable:
                case (int)FoodModel.CreepvinePiece:
                case (int)FoodModel.JellyPlant:
                case (int)FoodModel.KooshChunk:
                case (int)FoodModel.HangingFruit:
                case (int)FoodModel.Melon:
                case (int)FoodModel.NutrientBlock:
                case (int)FoodModel.CookedPeeper:
                case (int)FoodModel.CookedHoleFish:
                case (int)FoodModel.CookedGarryFish:
                case (int)FoodModel.CookedReginald:
                case (int)FoodModel.CookedBladderfish:
                case (int)FoodModel.CookedHoverfish:
                case (int)FoodModel.CookedSpadefish:
                case (int)FoodModel.CookedBoomerang:
                case (int)FoodModel.CookedEyeye:
                case (int)FoodModel.CookedOculus:
                case (int)FoodModel.CookedHoopfish:
                case (int)FoodModel.CookedSpinefish:
                case (int)FoodModel.CookedLavaEyeye:
                case (int)FoodModel.CookedLavaBoomerang:
                case (int)FoodModel.CuredPeeper:
                case (int)FoodModel.CuredHoleFish:
                case (int)FoodModel.CuredGarryFish:
                case (int)FoodModel.CuredReginald:
                case (int)FoodModel.CuredBladderfish:
                case (int)FoodModel.CuredHoverfish:
                case (int)FoodModel.CuredSpadefish:
                case (int)FoodModel.CuredBoomerang:
                case (int)FoodModel.CuredEyeye:
                case (int)FoodModel.CuredOculus:
                case (int)FoodModel.CuredHoopfish:
                case (int)FoodModel.CuredSpinefish:
                case (int)FoodModel.CuredLavaEyeye:
                case (int)FoodModel.CuredLavaBoomerang:
                    return true;
                default:
                    return false;
            }
        }

        internal const short MaxValue = 100;
        internal const short MinValue = -99;

        public new const string TypeName = "CustomFood";

        protected const string FoodModelKey = "FoodType";
        protected const string FoodKey = "FoodValue";
        protected const string WaterKey = "WaterValue";
        protected const string DecayRateKey = "DecayRateMod";
        protected const string OverfillKey = "AllowOverfill";

        protected readonly EmProperty<FoodModel> foodModel;
        protected readonly EmProperty<short> foodValue;
        protected readonly EmProperty<short> waterValue;
        protected readonly EmProperty<float> decayrate;
        protected readonly EmYesNo allowOverfill;

        public FoodModel FoodType
        {
            get => foodModel.Value;
            set => foodModel.Value = value;
        }

        public short FoodValue
        {
            get => foodValue.Value;
            set => foodValue.Value = value;
        }

        public short WaterValue
        {
            get => waterValue.Value;
            set => waterValue.Value = value;
        }

        public float DecayRateMod
        {
            get => decayrate.Value;
            set => decayrate.Value = value;
        }

        public bool AllowOverfill
        {
            get => allowOverfill.Value;
            set => allowOverfill.Value = value;
        }

        internal bool Decomposes => this.DecayRateMod > 0f;

        protected static List<EmProperty> CustomFoodProperties => new List<EmProperty>(AliasRecipeProperties)
        {
            new EmProperty<FoodModel>(FoodModelKey, FoodModel.None) { Optional = true },
            new EmProperty<TechType>(SpriteItemIdKey, TechType.None) { Optional = true },
            new EmProperty<short>(FoodKey, 0) { Optional = false },
            new EmProperty<short>(WaterKey, 0) { Optional = false },
            new EmProperty<float>(DecayRateKey, 0) { Optional = true },
            new EmYesNo(OverfillKey, true) { Optional = true },
        };

        internal TechType FoodPrefab
        {
            get
            {
                if (this.FoodType == FoodModel.None)
                {
                    if (this.FoodValue >= this.WaterValue)
                        return TechType.NutrientBlock;
                    else
                        return TechType.FilteredWater;
                }

                return (TechType)this.FoodType;
            }
        }

        internal string IconName
        {
            get
            {
                string imagePath = IOPath.Combine(FileLocations.AssetsFolder, $"{this.ItemID}.png");

                if (File.Exists(imagePath))
                    return imagePath;

                if (this.FoodValue >= this.WaterValue)
                    return "cake.png";
                else
                    return "juice.png";
            }
        }

        public CustomFood() : this(TypeName, CustomFoodProperties)
        {
        }

        protected CustomFood(string key) : this(key, CustomFoodProperties)
        {
        }

        protected CustomFood(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            foodValue = (EmProperty<short>)Properties[FoodKey];
            waterValue = (EmProperty<short>)Properties[WaterKey];
            decayrate = (EmProperty<float>)Properties[DecayRateKey];
            allowOverfill = (EmYesNo)Properties[OverfillKey];

            techGroup.Value = TechGroup.Survival;
            techCategory.DefaultValue = TechCategory.CookedFood;
        }

        internal override EmProperty Copy()
        {
            return new CustomFood(this.Key, this.CopyDefinitions);
        }

        public override bool PassesPreValidation()
        {
            return base.PassesPreValidation() & ValidateCustomFoodValues();
        }

        private bool ValidateCustomFoodValues()
        {
            if (this.FoodValue < MinValue || this.FoodValue > MaxValue)
            {
                QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} has {FoodKey} values out of range. Must be between {MinValue} and {MaxValue}. Entry will be discarded.");
                return false;
            }

            if (this.WaterValue < MinValue || this.WaterValue > MaxValue)
            {
                QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} has {WaterKey} values out of range. Must be between {MinValue} and {MaxValue}. Entry will be discarded.");
                return false;
            }

            return true;
        }

        protected override void HandleCustomSprite()
        {
            string imagePath = IOPath.Combine(FileLocations.AssetsFolder, $"{this.ItemID}.png");

            if (File.Exists(imagePath))
            {
                QuickLogger.Debug($"Custom sprite found in Assets folder for {this.Key} '{this.ItemID}' from {this.Origin}");
                SpriteHandler.RegisterSprite(this.TechType, ImageUtils.LoadSpriteFromFile(imagePath));
                return;
            }

            if (this.SpriteItemID > TechType.None && this.SpriteItemID < TechType.Databox)
            {
                QuickLogger.Debug($"{SpriteItemIdKey} '{this.SpriteItemID}' used for {this.Key} '{this.ItemID}' from {this.Origin}");
                SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get(this.SpriteItemID));
                return;
            }

            // TODO - Handle more custom icons

            if (this.FoodValue >= this.WaterValue)
            {
                SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get(TechType.NutrientBlock));
                return;
            }
            else
            {
                SpriteHandler.RegisterSprite(this.TechType, SpriteManager.Get(TechType.FilteredWater));
                return;
            }

            //QuickLogger.Warning($"No sprite loaded for {this.Key} '{this.ItemID}' from {this.Origin}");
        }

        protected override void HandleCustomPrefab()
        {
            if (this.TechType == TechType.None)
                throw new InvalidOperationException("TechTypeHandler.AddTechType must be called before PrefabHandler.RegisterPrefab.");

            PrefabHandler.RegisterPrefab(new CustomFoodPrefab(this));
        }
    }
}