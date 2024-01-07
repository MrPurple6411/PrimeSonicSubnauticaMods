#if !UNITY_EDITOR && (SUBNAUTICA || BELOWZERO)
namespace CustomCraft3.Serialization.Entries
{
    using System;
    using Common;
    using CustomCraft3.Interfaces;
    using CustomCraft3.Interfaces.InternalUse;
    using CustomCraft3.Serialization.Components;
    using Nautilus.Handlers;

    internal partial class CustomSize : EmTechTyped, ICustomSize, ICustomCraft
    {
        public override bool PassesPreValidation(OriginFile originFile)
        {
            return ValidateSizes() & base.PassesPreValidation(originFile);
        }

        private bool ValidateSizes()
        {
            if (this.Width < Min || this.Height < Min || this.Width > Max || this.Height > Max)
            {
                if (PassedPreValidation)
                    QuickLogger.Error($"Error in {this.Key} for '{this.ItemID}' from {this.Origin}. Size values must be between between {Min} and {Max}.");
                return false;
            }

            return true;
        }

        public bool SendToNautilus()
        {
            try
            {
                CraftDataHandler.SetItemSize(this.TechType, this.Width, this.Height);
                QuickLogger.Debug($"'{this.ItemID}' from {this.Origin} was resized to {this.Width}x{this.Height}");
                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {this.Key} '{this.ItemID}' from {this.Origin}", ex);
                return false;
            }
        }
    }
}
#endif