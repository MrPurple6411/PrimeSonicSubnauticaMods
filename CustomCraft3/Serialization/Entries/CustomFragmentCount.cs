#if !UNITY_EDITOR
namespace CustomCraft3.Serialization.Entries
{
    using System;
    using Common;
    using CustomCraft3.Interfaces;
    using CustomCraft3.Interfaces.InternalUse;
    using CustomCraft3.Serialization.Components;
    using Nautilus.Handlers;

    internal partial class CustomFragmentCount : EmTechTyped, ICustomFragmentCount, ICustomCraft
    {
        public bool SendToNautilus()
        {
            try
            {
                int fragCount = this.FragmentsToScan;
                if (fragCount < PDAScanner.EntryData.minFragments ||
                    fragCount > PDAScanner.EntryData.maxFragments)
                {
                    QuickLogger.Warning($"Invalid number of FragmentsToScan for entry '{this.ItemID}'. Must be between {PDAScanner.EntryData.minFragments} and {PDAScanner.EntryData.maxFragments}.");
                    return false;
                }

                if (this.TechType > TechType.Databox)
                {
                    QuickLogger.Warning($"{this.Key} entry '{this.ItemID}' from {this.Origin} appears to be a modded item. {this.Key} can only be applied to existing game items.");
                    return false;
                }

                PDAHandler.EditFragmentsToScan(this.TechType, fragCount);
                QuickLogger.Debug($"'{this.ItemID}' from {this.Origin} now requires {fragCount} fragments scanned to unlock.");
                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling Custom Fragment Count '{this.ItemID}' from {this.Origin}", ex);
                return false;
            }
        }
    }
}
#endif