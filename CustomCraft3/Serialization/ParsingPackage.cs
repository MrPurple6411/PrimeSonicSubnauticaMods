namespace CustomCraft3.Serialization;

using System.Collections.Generic;
using Common;
using CustomCraft3.Interfaces.InternalUse;
using CustomCraft3.Serialization.Components;
using CustomCraft3.Serialization.Entries;
using EasyMarkup;

internal class ParsingPackage<CustomCraftEntry, EmCollectionListT> : IParsingPackage
        where CustomCraftEntry : EmPropertyCollection, ICustomCraft, new()
        where EmCollectionListT : EmPropertyCollectionList<CustomCraftEntry>, new()
{
    public string ListKey { get; }

    internal IList<CustomCraftEntry> ParsedEntries { get; } = new List<CustomCraftEntry>();

    internal IList<CustomCraftEntry> SecondPassEntries { get; } = new List<CustomCraftEntry>();

    internal IDictionary<string, CustomCraftEntry> UniqueEntries { get; } = new Dictionary<string, CustomCraftEntry>();

    public string TypeName { get; } = typeof(CustomCraftEntry).Name;
    public string[] TutorialText { get; } = (new CustomCraftEntry()).TutorialText;

    public ParsingPackage(string listKey)
    {
        this.ListKey = listKey;
    }

    public int ParseEntries(string serializedData, OriginFile file)
    {
        var list = new EmCollectionListT();

        bool successfullyParsed = list.Deserialize(serializedData);

        if (!successfullyParsed)
            return -1; // Error case

        if (list.Count == 0)
            return 0; // No entries

        int count = 0;
        foreach (CustomCraftEntry item in list)
        {
            item.Origin = file;
            this.ParsedEntries.Add(item);
            count++;
        }

        return count; // Return the number of unique entries added in this list
    }

    public void PrePassValidation()
    {
        QuickLogger.Debug($"Prepass validation for {this.TypeName} entries");
        int successCount = 0;
        foreach (CustomCraftEntry item in this.ParsedEntries)
        {
            if (!item.PassesPreValidation(item.Origin))
            {
                if (!SecondPassEntries.Contains(item))
                    SecondPassEntries.Add(item);
                else
                    QuickLogger.Warning($"Duplicate entry for {this.TypeName} '{item.ID}' in {item.Origin} was already added by another working file. Kept first one. Discarded duplicate.");
                continue;
            }

            if (this.UniqueEntries.ContainsKey(item.ID))
            {
                QuickLogger.Warning($"Duplicate entry for {this.TypeName} '{item.ID}' in {item.Origin} was already added by another working file. Kept first one. Discarded duplicate.");
            }
            else
            {
                // All checks passed
                this.UniqueEntries.Add(item.ID, item);
                successCount++;
            }
        }
    }

    /// <summary>
    /// This is a second pass validation that is only run if the first pass validation fails due to files being loaded in the wrong order.
    /// </summary>
    public void SecondPassValidation()
    {
        if (this.SecondPassEntries.Count == 0)
        {
            if (ParsedEntries.Count > 0)
                QuickLogger.Info($"{this.UniqueEntries.Count} of {this.ParsedEntries.Count} {this.TypeName} entries staged for patching");

            return; // Nothing to do here
        }

        QuickLogger.Debug($"Second pass validation for {this.TypeName} entries");
        foreach (CustomCraftEntry item in this.SecondPassEntries)
        {
            if (!item.PassesPreValidation(item.Origin))
            {
                QuickLogger.Debug($"Entry for '{item.ID}' from file '{item.Origin}' will be discarded.");
                continue;
            }

            if (this.UniqueEntries.ContainsKey(item.ID))
            {
                QuickLogger.Warning($"Duplicate entry for {this.TypeName} '{item.ID}' in {item.Origin} was already added by another working file. Kept first one. Discarded duplicate.");
            }
            else
            {
                // All checks passed
                this.UniqueEntries.Add(item.ID, item);
            }
        }

        QuickLogger.Info($"{this.UniqueEntries.Count} of {this.ParsedEntries.Count} {this.TypeName} entries staged for patching");
    }

    public void SendToNautilus()
    {
        QuickLogger.Debug($"Sending {this.TypeName} entries to Nautilus");
        int successCount = 0;
        foreach (CustomCraftEntry item in this.UniqueEntries.Values)
        {
            if (!item.PassedSecondValidation)
            {
                QuickLogger.Warning($"Entry for '{item.ID}' from file '{item.Origin}' failed secondary checks for duplicate IDs. It will not be patched.");
            }
            else if (item.SendToNautilus())
            {
                successCount++;
            }
        }

        if (this.UniqueEntries.Count > 0)
            QuickLogger.Info($"{successCount} of {this.UniqueEntries.Count} {this.TypeName} entries were patched");
    }
}
