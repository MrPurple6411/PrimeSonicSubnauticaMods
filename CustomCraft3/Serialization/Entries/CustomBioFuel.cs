#if !UNITY_EDITOR && (SUBNAUTICA || BELOWZERO)
namespace CustomCraft3.Serialization.Entries
{
    using System;
    using Common;
    using CustomCraft3.Interfaces;
    using CustomCraft3.Interfaces.InternalUse;
    using CustomCraft3.Serialization.Components;

    internal partial class CustomBioFuel : EmTechTyped, ICustomBioFuel, ICustomCraft
    {
        public bool SendToNautilus()
        {
            try
            {
                BaseBioReactor.charge[this.TechType] = this.Energy;
                QuickLogger.Debug($"'{this.ItemID}' now provides {this.Energy} energy in the BioReactor - Entry from {this.Origin}");
                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling Modified Recipe '{this.ItemID} from {this.Origin}'", ex);
                return false;
            }
        }
    }
}
#endif