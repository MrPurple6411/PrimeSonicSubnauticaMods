namespace CustomCraft3.Serialization.Components
{
    using System.Collections.Generic;
    using Common;
    using EasyMarkup;
    using static CraftData;

    internal class EmIngredient : EmTechTyped
    {
        public const short Max = 25;
        public const short Min = 1;
        private const string RequiredKey = "Required";

        private readonly EmProperty<short> required;

        public short Required
        {
            get => required.Value;
            set => required.Value = value;
        }

        protected static List<EmProperty> IngredientProperties => new List<EmProperty>(TechTypedProperties)
    {
        new EmProperty<short>(RequiredKey, 1),
    };

        internal EmIngredient(string item) : this()
        {
            this.ItemID = item;
        }

        internal EmIngredient(TechType item) : this(item.ToString())
        {
            this.TechType = item;
        }

        internal EmIngredient(string item, short required) : this(item)
        {
            this.Required = required;
        }

        internal EmIngredient(TechType item, short required) : this(item.ToString(), required)
        {
            this.TechType = item;
        }

        public EmIngredient() : base("Ingredient", IngredientProperties)
        {
            required = (EmProperty<short>)Properties[RequiredKey];
        }

        public override EmProperty Copy()
        {
            return new EmIngredient(this.ItemID, this.Required);
        }

        public override bool PassesPreValidation(OriginFile originFile)
        {
            return RequireValueInRange() & base.PassesPreValidation(originFile);
        }

        private bool RequireValueInRange()
        {
            if (this.Required > Max || this.Required < Min)
            {
                if (!PassedPreValidation)
                    QuickLogger.Error($"Error in {this.Key} {RequiredKey} for '{this.ItemID}'. Required values must be between between {Min} and {Max}.");
                return false;
            }

            return true;
        }

        public Ingredient ToIngredient()
        {
            return new Ingredient(this.TechType, this.Required);
        }
    }
}