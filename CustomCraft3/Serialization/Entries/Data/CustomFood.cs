namespace CustomCraft3.Serialization.Entries
{
    using System.Collections.Generic;
    using CustomCraft3.Serialization.Lists;
    using EasyMarkup;

    internal partial class CustomFood : AliasRecipe
    {
        public override string[] TutorialText => CustomFoodTutorialText;

        internal static readonly string[] CustomFoodTutorialText = new[]
        {
        $"{CustomFoodList.ListKey}: A powerful tool to satisfy the insatiable.",
        "    Custom foods allow you to create custom eatables, for example Hamburgers or Sodas.",
        $"    {CustomFoodList.ListKey} have all the same properties of {AliasRecipeList.ListKey}, but when creating your own items, you will want to include these new properties:",
        $"        {FoodKey}: Defines how much food the user will gain on consumption.",
        $"            Must be between {MinValue} and {MaxValue}.",
        $"        {WaterKey}: Defines how much water the user will gain on consumption.",
        $"            Must be between {MinValue} and {MaxValue}.",
        $"        {OxygenKey}: Defines how much oxygen the user will gain on consumption.",
        $"            Must be between {MinValue} and {MaxValue}.",
        $"        {HealthKey}: Defines how much health the user will gain on consumption.",
        $"            Must be between {MinValue} and {MaxValue}.",
#if BELOWZERO
        $"        {HeatKey}: Defines how much water the user will gain on consumption.",
        $"            Must be between {MinValue} and {MaxValue}.",
#endif
        $"        {DecayRateKey}: An optional property that defines the speed at which food will decompose.",
        "            If set to 1 it will decompose as fast as any cooked fish." +
        "            If set to 2 it will decay twice as fast.",
        "            If set to 0.5 it will decay half as fast.",
        "            And if set to 0 it will not decay.",
        "            Without this property, the food item will not decay.",
        $"        {FoodModelKey}: Sets the model the food should use in the fabricator and upon being dropped. This needs to be an already existing food or drink.",
        "            Leaving out this property results in an automatic definition based on the food and water values.",
        $"        {UseDrinkSoundKey}: An optional property that sets whether the drinking sound effect should be used when consuming this item.",
        $"            Set to 'NO' to force the eating sound to be used, regardless of the {WaterKey}.",
        $"            Set to 'YES' to force the drinking sound to be used, regardless of the {FoodKey}.",
        $"            If not set, then the drinking sound will be used if {WaterKey} is 5 times greater than {FoodKey}",
    };

        // We may need this later.
        internal static bool IsMappedFoodType(TechType techType)
        {
            switch ((int)techType)
            {
                case (int)FoodModel.None:
                case (int)FoodModel.BigFilteredWater:
                case (int)FoodModel.DisinfectedWater:
                case (int)FoodModel.FilteredWater:
                case (int)FoodModel.WaterFiltrationSuitWater:
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

        internal const short MaxValue = 200;
        internal const short MinValue = -199;

        public new const string TypeName = "CustomFood";

        protected const string FoodModelKey = "FoodType";
        protected const string FoodKey = "FoodValue";
        protected const string WaterKey = "WaterValue";
        protected const string OxygenKey = "OxygenValue";
        protected const string HealthKey = "HealthValue";
        protected const string HeatKey = "HeatValue";
        protected const string DecayRateKey = "DecayRateMod";
        protected const string UseDrinkSoundKey = "UseDrinkSound";

        protected readonly EmProperty<FoodModel> foodModel;
        protected readonly EmProperty<short> foodValue;
        protected readonly EmProperty<short> waterValue;
        protected readonly EmProperty<short> oxygenValue;
        protected readonly EmProperty<short> healthValue;
        protected readonly EmProperty<short> heatValue;
        protected readonly EmProperty<float> decayrate;
        protected readonly EmYesNo useDrinkSound;

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

        public short OxygenValue
        {
            get => oxygenValue.Value;
            set => oxygenValue.Value = value;
        }

        public short HealthValue
        {
            get => healthValue.Value;
            set => healthValue.Value = value;
        }

        public short HeatValue
        {
            get => heatValue.Value;
            set => heatValue.Value = value;
        }

        public float DecayRateMod
        {
            get => decayrate.Value;
            set => decayrate.Value = value;
        }

        public bool UseDrinkSound
        {
            get
            {
                if (useDrinkSound.HasValue)
                    return useDrinkSound.Value;
                else
                    return this.FoodValue * 5f <= this.WaterValue;
            }
            set => useDrinkSound.Value = value;
        }

        internal bool Decomposes => this.DecayRateMod > 0f;

        protected static List<EmProperty> CustomFoodProperties => new List<EmProperty>(AliasRecipeProperties)
    {
        new EmProperty<FoodModel>(FoodModelKey, FoodModel.None) { Optional = true },
        new EmProperty<TechType>(SpriteItemIdKey, TechType.None) { Optional = true },
        new EmProperty<short>(FoodKey, 0) { Optional = false },
        new EmProperty<short>(WaterKey, 0) { Optional = false },
        new EmProperty<short>(OxygenKey, 0) { Optional = true },
        new EmProperty<short>(HealthKey, 0) { Optional = true },
        new EmProperty<short>(HeatKey, 0) { Optional = true },
        new EmProperty<float>(DecayRateKey, 0) { Optional = true },
        new EmYesNo(UseDrinkSoundKey, false) { Optional = true },
    };

        internal TechType FoodPrefab
        {
            get
            {
                if (this.FoodType == FoodModel.None)
                {
                    if (this.FoodValue >= this.WaterValue)
                    {
                        return TechType.NutrientBlock;
                    }
                    else
                    {
                        return TechType.FilteredWater;
                    }
                }

                if (IsMappedFoodType((TechType)this.FoodType))
                {
                    return (TechType)this.FoodType;
                }
                else
                {
                    return TechType.NutrientBlock;
                }
            }
        }

        internal string DefaultIconFileName
        {
            get
            {
                int index = (this.ItemID.GetHashCode() % 8) + 1;

                if (this.FoodValue >= this.WaterValue)
                    return $"cake{index}.png";
                else
                    return $"juice{index}.png";
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
            oxygenValue = (EmProperty<short>)Properties[OxygenKey];
            healthValue = (EmProperty<short>)Properties[HealthKey];
            heatValue = (EmProperty<short>)Properties[HeatKey];
            decayrate = (EmProperty<float>)Properties[DecayRateKey];
            useDrinkSound = (EmYesNo)Properties[UseDrinkSoundKey];

            techGroup.Value = TechGroup.Survival;
#if SUBNAUTICA
            techCategory.DefaultValue = TechCategory.CookedFood;
#elif BELOWZERO
            techCategory.DefaultValue = TechCategory.FoodAndDrinks;
#endif
            foodModel = (EmProperty<FoodModel>)Properties[FoodModelKey];

            amountCrafted.DefaultValue = 1;
        }
        public override EmProperty Copy()
        {
            return new CustomFood(this.Key, this.CopyDefinitions);
        }
    }
}