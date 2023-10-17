namespace CustomCraft3Tests;

using System;
using CustomCraft3.Serialization;
using CustomCraft3.Serialization.Components;
using CustomCraft3.Serialization.Entries;
using CustomCraft3.Serialization.Lists;
using Nautilus.Assets.PrefabTemplates;
using NUnit.Framework;

[TestFixture]
internal class CustomFabricatorTests
{
    private const string fabricatorID = "PlaceablesFabricator";
    private const string scienceTab = "ScienceTab";

    [Test]
    public void SampleFabricator()
    {
        CustomFabricator originalFabricator = GetSampleFabricator();

        string originalSerialized = originalFabricator.PrettyPrint();
        Console.WriteLine(originalSerialized);

        var parsedFabricator = new CustomFabricator();
        Assert.IsTrue(parsedFabricator.FromString(originalSerialized));

        Assert.AreEqual(originalFabricator.ItemID, parsedFabricator.ItemID);
        Assert.AreEqual(originalFabricator.AllowedInBase, parsedFabricator.AllowedInBase);
        Assert.AreEqual(originalFabricator.AllowedInCyclops, parsedFabricator.AllowedInCyclops);
        Assert.AreEqual(originalFabricator.Model, parsedFabricator.Model);
        Assert.AreEqual(originalFabricator.DisplayName, parsedFabricator.DisplayName);
        Assert.AreEqual(originalFabricator.Tooltip, parsedFabricator.Tooltip);

        Assert.AreEqual(originalFabricator.EmIngredients.Count, parsedFabricator.EmIngredients.Count);
        Assert.AreEqual(originalFabricator.EmIngredients[0].ItemID, parsedFabricator.EmIngredients[0].ItemID);
        Assert.AreEqual(originalFabricator.EmIngredients[0].Required, parsedFabricator.EmIngredients[0].Required);
        Assert.AreEqual(originalFabricator.EmIngredients[1].ItemID, parsedFabricator.EmIngredients[1].ItemID);
        Assert.AreEqual(originalFabricator.EmIngredients[1].Required, parsedFabricator.EmIngredients[1].Required);

        Assert.AreEqual(originalFabricator.ForceUnlockAtStart, parsedFabricator.ForceUnlockAtStart);
        Assert.AreEqual(originalFabricator.PdaCategory, parsedFabricator.PdaCategory);
        Assert.AreEqual(originalFabricator.PdaGroup, parsedFabricator.PdaGroup);

        Assert.AreEqual(originalFabricator.CustomCraftingTabs.Count, parsedFabricator.CustomCraftingTabs.Count);
        Assert.AreEqual(originalFabricator.CustomCraftingTabs[0].TabID, parsedFabricator.CustomCraftingTabs[0].TabID);
        Assert.AreEqual(originalFabricator.CustomCraftingTabs[0].DisplayName, parsedFabricator.CustomCraftingTabs[0].DisplayName);
        Assert.AreEqual(originalFabricator.CustomCraftingTabs[0].SpriteItemID, parsedFabricator.CustomCraftingTabs[0].SpriteItemID);

        Assert.AreEqual(originalFabricator.AddedRecipes.Count, parsedFabricator.AddedRecipes.Count);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].ItemID, parsedFabricator.AddedRecipes[0].ItemID);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].AmountCrafted, parsedFabricator.AddedRecipes[0].AmountCrafted);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].ForceUnlockAtStart, parsedFabricator.AddedRecipes[0].ForceUnlockAtStart);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].Path, parsedFabricator.AddedRecipes[0].Path);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].EmIngredients[0].ItemID, parsedFabricator.AddedRecipes[0].EmIngredients[0].ItemID);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].EmIngredients[0].Required, parsedFabricator.AddedRecipes[0].EmIngredients[0].Required);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].EmIngredients[1].ItemID, parsedFabricator.AddedRecipes[0].EmIngredients[1].ItemID);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].EmIngredients[1].Required, parsedFabricator.AddedRecipes[0].EmIngredients[1].Required);

        string parsedSerialized = parsedFabricator.PrettyPrint();
        Console.WriteLine(parsedSerialized);
        Assert.AreEqual(originalSerialized, parsedSerialized);
    }

    [Test]
    public void SampleFabricatorList()
    {
        CustomFabricator originalFabricator = GetSampleFabricator();

        var originalList = new CustomFabricatorList()
        {
            originalFabricator
        };

        string originalSerialized = originalList.PrettyPrint();
        Console.WriteLine(originalSerialized);

        var parsingList = new CustomFabricatorList();

        Assert.IsTrue(parsingList.FromString(originalSerialized));

        CustomFabricator parsedFabricator = parsingList[0];

        Assert.AreEqual(originalFabricator.ItemID, parsedFabricator.ItemID);
        Assert.AreEqual(originalFabricator.AllowedInBase, parsedFabricator.AllowedInBase);
        Assert.AreEqual(originalFabricator.AllowedInCyclops, parsedFabricator.AllowedInCyclops);
        Assert.AreEqual(originalFabricator.Model, parsedFabricator.Model);
        Assert.AreEqual(originalFabricator.DisplayName, parsedFabricator.DisplayName);
        Assert.AreEqual(originalFabricator.Tooltip, parsedFabricator.Tooltip);

        Assert.AreEqual(originalFabricator.EmIngredients.Count, parsedFabricator.EmIngredients.Count);
        Assert.AreEqual(originalFabricator.EmIngredients[0].ItemID, parsedFabricator.EmIngredients[0].ItemID);
        Assert.AreEqual(originalFabricator.EmIngredients[0].Required, parsedFabricator.EmIngredients[0].Required);
        Assert.AreEqual(originalFabricator.EmIngredients[1].ItemID, parsedFabricator.EmIngredients[1].ItemID);
        Assert.AreEqual(originalFabricator.EmIngredients[1].Required, parsedFabricator.EmIngredients[1].Required);

        Assert.AreEqual(originalFabricator.ForceUnlockAtStart, parsedFabricator.ForceUnlockAtStart);
        Assert.AreEqual(originalFabricator.PdaCategory, parsedFabricator.PdaCategory);
        Assert.AreEqual(originalFabricator.PdaGroup, parsedFabricator.PdaGroup);

        Assert.AreEqual(originalFabricator.CustomCraftingTabs.Count, parsedFabricator.CustomCraftingTabs.Count);
        Assert.AreEqual(originalFabricator.CustomCraftingTabs[0].TabID, parsedFabricator.CustomCraftingTabs[0].TabID);
        Assert.AreEqual(originalFabricator.CustomCraftingTabs[0].DisplayName, parsedFabricator.CustomCraftingTabs[0].DisplayName);
        Assert.AreEqual(originalFabricator.CustomCraftingTabs[0].SpriteItemID, parsedFabricator.CustomCraftingTabs[0].SpriteItemID);

        Assert.AreEqual(originalFabricator.AddedRecipes.Count, parsedFabricator.AddedRecipes.Count);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].ItemID, parsedFabricator.AddedRecipes[0].ItemID);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].AmountCrafted, parsedFabricator.AddedRecipes[0].AmountCrafted);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].ForceUnlockAtStart, parsedFabricator.AddedRecipes[0].ForceUnlockAtStart);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].Path, parsedFabricator.AddedRecipes[0].Path);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].EmIngredients[0].ItemID, parsedFabricator.AddedRecipes[0].EmIngredients[0].ItemID);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].EmIngredients[0].Required, parsedFabricator.AddedRecipes[0].EmIngredients[0].Required);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].EmIngredients[1].ItemID, parsedFabricator.AddedRecipes[0].EmIngredients[1].ItemID);
        Assert.AreEqual(originalFabricator.AddedRecipes[0].EmIngredients[1].Required, parsedFabricator.AddedRecipes[0].EmIngredients[1].Required);

        string parsedSerialized = parsingList.PrettyPrint();
        Console.WriteLine(parsedSerialized);
        Assert.AreEqual(originalSerialized, parsedSerialized);
    }

    private static CustomFabricator GetSampleFabricator()
    {
        return new CustomFabricator
        {
            AllowedInBase = true,
            AllowedInCyclops = true,
            Model = FabricatorTemplate.Model.Workbench,
            ItemID = fabricatorID,
            DisplayName = "Simple Decorations Fabricator",
            Tooltip = "A sample fabricator for creating the game's original decorations",
            EmIngredients =
            {
                new EmIngredient(TechType.ComputerChip),
                new EmIngredient(TechType.Titanium, 2),
            },
            ForceUnlockAtStart = true,
            PdaCategory = TechCategory.InteriorModule,
            PdaGroup = TechGroup.InteriorModules,
            Origin = new OriginFile("CodeGenerated"),
            CustomCraftingTabs =
            {
                new CfCustomCraftingTab
                {
                    TabID = scienceTab,
                    DisplayName = "Science!",
                    ParentTabPath = fabricatorID,
                    SpriteItemID = TechType.LabEquipment3,
                }
            },
            AddedRecipes =
            {
                new CfAddedRecipe
                {
                    ItemID = TechType.LabEquipment1.ToString(),
                    AmountCrafted = 1,
                    ForceUnlockAtStart = true,
                    EmIngredients =
                    {
                        new EmIngredient(TechType.Glass),
                        new EmIngredient(TechType.Titanium),
                    },
                    Path = $"{fabricatorID}/{scienceTab}"
                }
            }
        };
    }
}
