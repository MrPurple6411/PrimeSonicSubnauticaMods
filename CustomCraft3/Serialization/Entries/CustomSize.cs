namespace CustomCraft3.Serialization.Entries;

using System;
using System.Collections.Generic;
using Common;
using CustomCraft3.Interfaces;
using CustomCraft3.Interfaces.InternalUse;
using CustomCraft3.Serialization.Components;
using CustomCraft3.Serialization.Lists;
using EasyMarkup;
using Nautilus.Handlers;

internal class CustomSize : EmTechTyped, ICustomSize, ICustomCraft
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

    internal override EmProperty Copy()
    {
        return new CustomSize(this.Key, this.CopyDefinitions);
    }

    public override bool PassesPreValidation(OriginFile originFile)
    {
        return  ValidateSizes() & base.PassesPreValidation(originFile);
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
