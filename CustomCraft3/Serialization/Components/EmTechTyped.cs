namespace CustomCraft3.Serialization.Components
{
    using System.Collections.Generic;
    using Common;
    using CustomCraft3.Interfaces;
    using CustomCraft3.Serialization;
    using EasyMarkup;

#if !UNITY_EDITOR && (SUBNAUTICA || BELOWZERO)
    using Nautilus.Handlers;
#endif

    internal abstract partial class EmTechTyped : EmPropertyCollection, ITechTyped
    {
        protected const string ItemIdKey = "ItemID";

        protected readonly EmProperty<string> emTechType;

        protected static List<EmProperty> TechTypedProperties => new List<EmProperty>(1)
    {
        new EmProperty<string>(ItemIdKey),
    };

        public EmTechTyped() : this("TechTyped", TechTypedProperties)
        {
        }

        protected EmTechTyped(string key) : this(key, TechTypedProperties)
        {
        }

        protected EmTechTyped(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emTechType = (EmProperty<string>)Properties[ItemIdKey];
        }

        public virtual string ItemID
        {
            get => emTechType.Value;
            set => emTechType.Value = value;
        }

        public TechType TechType { get; set; } = TechType.None;

        public bool PassedPreValidation { get; set; } = true;

        public virtual bool PassesPreValidation(OriginFile originFile)
        {
            // Now we can safely do the prepass check in case we need to create a new modded TechType
            this.TechType = GetTechType(this.ItemID);

            if (this.TechType == TechType.None)
            {
                QuickLogger.Warning($"Could not resolve {ItemIdKey} value of '{this.ItemID}' for '{this.Key}' from file '{originFile}'. Discarded entry.");
                PassedPreValidation = false;
                return false;
            }

            return true;
        }

        protected static TechType GetTechType(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return TechType.None;
            }

            // Look for a known TechType
            if (TechTypeExtensions.FromString(value, out TechType tType, true))
            {
                return tType;
            }

#if !UNITY_EDITOR && (SUBNAUTICA || BELOWZERO)
            //  Not one of the known TechTypes - is it registered with Nautilus?
            if (EnumHandler.TryGetValue(value, out TechType custom))
            {
                return custom;
            }
#endif

            return TechType.None;
        }

        protected void AddCraftNode(CraftTreePath newPath, TechType techType, OriginFile origin)
        {

#if !UNITY_EDITOR && (SUBNAUTICA || BELOWZERO)
            if (newPath.IsAtRoot)
            {
                QuickLogger.Debug($"New crafting node for {this.Key} '{newPath.FinalNodeID}' from {origin} added at root");
                CraftTreeHandler.AddCraftingNode(newPath.Scheme, techType);
                return;
            }

            QuickLogger.Debug($"New crafting node for {this.Key} '{newPath.FinalNodeID}' from {origin} added at '{string.Join("/", newPath.StepsToParentTab)}'");
            CraftTreeHandler.AddCraftingNode(newPath.Scheme, techType, newPath.StepsToParentTab);
#endif
        }
    }
}