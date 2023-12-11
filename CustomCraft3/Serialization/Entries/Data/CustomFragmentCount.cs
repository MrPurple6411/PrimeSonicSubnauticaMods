namespace CustomCraft3.Serialization.Entries
{
    using System.Collections.Generic;
    using CustomCraft3.Interfaces.InternalUse;
    using CustomCraft3.Serialization.Components;
    using CustomCraft3.Serialization.Lists;
    using EasyMarkup;

    internal partial class CustomFragmentCount : EmTechTyped, ICustomCraft
    {
        private const string FragmentsToScanKey = "FragmentsToScan";
        private const string TypeName = "CustomFragments";

        public string[] TutorialText => CustomFragmentCountTutorial;

        internal static readonly string[] CustomFragmentCountTutorial = new[]
        {
        $"{CustomFragmentCountList.ListKey}: Change how many fragments must be scanned to unlock recipes/blueprints",
        $"    In addition to the usual {ItemIdKey}, you only need one more property for this one:",
        $"        {FragmentsToScanKey}: Simply set this to the total number of fragments that must be scanned to unlock the item in question.",
    };

        private readonly EmProperty<int> emFragmentCount;

        protected static List<EmProperty> FragmentProperties => new List<EmProperty>(TechTypedProperties)
    {
        new EmProperty<int>(FragmentsToScanKey, 1),
    };

        public CustomFragmentCount() : this(TypeName, FragmentProperties)
        {
        }

        protected CustomFragmentCount(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emFragmentCount = (EmProperty<int>)Properties[FragmentsToScanKey];
        }

        public OriginFile Origin { get; set; }

        public bool PassedSecondValidation => true;

        internal CustomFragmentCount(string itemID, int fragmentsToScan) : this()
        {
            this.ItemID = itemID;
            this.FragmentsToScan = fragmentsToScan;
        }

        public string ID => this.ItemID;

        public int FragmentsToScan
        {
            get => emFragmentCount.Value;
            set => emFragmentCount.Value = value;
        }
        public override EmProperty Copy()
        {
            return new CustomFragmentCount(this.Key, this.CopyDefinitions);
        }
    }
}