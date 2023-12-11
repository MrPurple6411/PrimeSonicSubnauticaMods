namespace CustomCraft3.Serialization.Entries
{
    using System.Collections.Generic;
    using CustomCraft3.Interfaces.InternalUse;
    using CustomCraft3.Serialization.Components;
    using CustomCraft3.Serialization.Lists;
    using EasyMarkup;

    internal partial class CustomSize : EmTechTyped, ICustomCraft
    {
        public string[] TutorialText => CustomSizeTutorial;

        internal static readonly string[] CustomSizeTutorial = new[]
        {
       $"{CustomSizeList.ListKey}: Customize the space occupied by an inventory item.",
       $"    Width: Must be a value between {Min} and {Max}",
       $"    Height: Must be a value between {Min} and {Max}",
    };

        public const short Max = 6;
        public const short Min = 1;

        private const string WidthKey = "Width";
        private const string HeightKey = "Height";

        protected readonly EmProperty<short> emWidth;
        protected readonly EmProperty<short> emHeight;

        public string ID => this.ItemID;

        public short Width
        {
            get => emWidth.Value;
            set
            {
                if (value > Max || value < Min)
                    value = emWidth.DefaultValue;

                emWidth.Value = value;
            }
        }

        public short Height
        {
            get => emHeight.Value;
            set
            {
                if (value > Max || value < Min)
                    value = emHeight.DefaultValue;

                emHeight.Value = value;
            }
        }

        protected static List<EmProperty> SizeProperties => new List<EmProperty>(TechTypedProperties)
    {
        new EmProperty<short>(WidthKey, 1),
        new EmProperty<short>(HeightKey, 1)
    };

        public OriginFile Origin { get; set; }

        public bool PassedSecondValidation => true;

        public CustomSize() : this("CustomSize", SizeProperties)
        {
        }

        protected CustomSize(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emWidth = (EmProperty<short>)Properties[WidthKey];
            emHeight = (EmProperty<short>)Properties[HeightKey];
        }

        public override EmProperty Copy()
        {
            return new CustomSize(this.Key, this.CopyDefinitions);
        }
    }
}