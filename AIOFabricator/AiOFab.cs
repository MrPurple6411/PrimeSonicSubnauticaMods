namespace AIOFabricator;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Common;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;
using static CraftData;

internal static class AiOFab
{
    private const string DisplayNameFormat = "{0}Menu_{1}";
    private const string TabSpriteFormat = "{0}_{1}";

    private const string AioFabScheme = "AiOFab";

    internal static Dictionary<CraftTree.Type, TechType> SpriteMap = new()
    {
        { CraftTree.Type.SeamothUpgrades, TechType.BaseUpgradeConsole },
        { CraftTree.Type.MapRoom, TechType.BaseMapRoom },
#if SUBNAUTICA
        { CraftTree.Type.CyclopsFabricator, TechType.Cyclops },
#endif
    };

    public static CraftTree.Type TreeTypeID { get; private set; }
    public static ModCraftTreeRoot Root { get; private set; }

    private static Texture2D texture;
    private static Texture2D spriteTexture;

    internal static void CreateAndRegister()
    {
        LoadImageFiles();

        var Info = PrefabInfo.WithTechType(AioFabScheme, "All-In-One Fabricator",
               "Multi-fuction fabricator capable of synthesizing most blueprints.", "English", false)
            .WithIcon(spriteTexture != null ? ImageUtils.LoadSpriteFromTexture(spriteTexture) : SpriteManager.Get(TechType.Fabricator));

        var prefab = new CustomPrefab(Info);

        if (GetBuilderIndex(TechType.Fabricator, out var group, out var category, out _))
        {
            var scanGadget = prefab.SetPdaGroupCategoryBefore(group, category, TechType.Fabricator)
                .WithAnalysisTech(spriteTexture == null ? null : Sprite.Create(spriteTexture, new Rect(0f, 0f, spriteTexture.width, spriteTexture.height), new Vector2(0.5f, 0.5f)), null, null);

            scanGadget.RequiredForUnlock =
#if SUBNAUTICA
                TechType.Cyclops;
#else
                TechType.SeaTruck;
#endif
        }

        var fabGadget = prefab.CreateFabricator(out var treeType);
        TreeTypeID = treeType;
        Root = fabGadget.Root;

        var aioFabTemplate = new FabricatorTemplate(Info, TreeTypeID)
        {
            ModifyPrefab = ModifyGameObject,
            FabricatorModel = FabricatorTemplate.Model.Fabricator,
            ConstructableFlags = ConstructableFlags.Wall | ConstructableFlags.Base | ConstructableFlags.Submarine
            | ConstructableFlags.Inside | ConstructableFlags.AllowedOnConstructable,
        };

        prefab.SetGameObject(aioFabTemplate);

        CraftDataHandler.SetRecipeData(Info.TechType, GetBlueprintRecipe());
        prefab.Register();

        Harmony.CreateAndPatchAll(typeof(AiOFab), MyPluginInfo.PLUGIN_GUID);
    }

    private static void LoadImageFiles()
    {
        string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string folderPath = Path.Combine(executingLocation, "Assets");

        if (texture == null)
        {
            string fileLocation = Path.Combine(folderPath, "AiOFabTex.png");
            texture = ImageUtils.LoadTextureFromFile(fileLocation);
        }

        if (spriteTexture == null)
        {
            string fileLocation = Path.Combine(folderPath, "AiOFab.png");
            spriteTexture = ImageUtils.LoadTextureFromFile(fileLocation);
        }
    }

    [HarmonyPatch(typeof(CraftTree), nameof(CraftTree.GetTree)), HarmonyPostfix, HarmonyPriority(Priority.Last)]
    private static void CreateCraftingTree(CraftTree.Type treeType, ref CraftTree __result)
    {
        if (treeType != TreeTypeID)
            return;

        CraftNode aioRoot = new CraftNode("Root");

        foreach (CraftTree.Type entry in Enum.GetValues(typeof(CraftTree.Type)))
        {
            // Skip the AIOFab tree and the Mobile vehicle bay
            if (entry == TreeTypeID || entry == CraftTree.Type.Constructor)
                continue;

#if BELOWZERO
            // Skip the SeaTruck and Cyclops fabricators
            if (entry == CraftTree.Type.SeaTruckFabricator || entry == CraftTree.Type.CyclopsFabricator)
                continue;
#endif
            try
            {
                CraftTree tree = CraftTree.GetTree(entry);

                // Skip the tree if it's null
                if (tree == null)
                    continue;

                string scheme = tree.id;
                if (!CloneTopLevelTab(entry, scheme))
                    continue;

                CraftNode root = tree.nodes;

                CloneTabDetails(scheme, root);
                aioRoot.AddNode(root);
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Error cloning crafttree '{entry}'{Environment.NewLine}{ex.Message}");
            }
        }

        __result = new CraftTree(AioFabScheme, aioRoot);
    }

    private static bool CloneTopLevelTab(CraftTree.Type entry, string scheme)
    {
        string lineId = string.Format(DisplayNameFormat, AioFabScheme, scheme);
        if (Language.main.TryGet(lineId, out _))
        {
            return true;
        }

        if (!SpriteMap.TryGetValue(entry, out TechType tabIconId) && !TechTypeExtensions.FromString(scheme, out tabIconId, true))
        {
            QuickLogger.Warning($"Problem cloning top level tab for '{scheme}', Unable to identify techtype from {scheme}");
            return false;
        }

        var sprite = SpriteManager.Get(tabIconId);

        if (sprite == SpriteManager.defaultSprite)
        {
            QuickLogger.Warning($"Problem cloning top level tab for '{scheme}', Sprite resource not found.");
            return false;
        }

        QuickLogger.Debug($"Cloning top level tab for '{scheme}'");
        string displayText = Language.main.GetOrFallback(scheme, scheme);
        switch (displayText)
        {
            case "MapRoom":
                displayText = "Scanner Upgrades";
                break;
            case "Fabricator" when entry == CraftTree.Type.CyclopsFabricator:
                displayText = "Cyclops Fabricator";
                break;
            case "SeamothUpgrades":
                displayText = "Vehicle Upgrade Console";
                break;
        }

        LanguageHandler.SetLanguageLine(lineId, displayText);
        SpriteHandler.RegisterSprite(SpriteManager.Group.Category, string.Format(TabSpriteFormat, AioFabScheme, scheme), sprite);
        return true;
    }

    private static void CloneTabDetails(string scheme, CraftNode node)
    {
        if (node == null)
            return;

        switch (node.action)
        {
            case TreeAction.Craft:
                return;
            case TreeAction.None:
                node.id = scheme;
                node.action = TreeAction.Expand;
                QuickLogger.Info($"Cloning tab nodes for '{scheme}:{node.id}'");
                break;
            case TreeAction.Expand:
            {
                string clonedLangKey = string.Format(DisplayNameFormat, AioFabScheme, node.id);

                if (Language.main.TryGet(clonedLangKey, out _))
                {
                    QuickLogger.Debug($"Skipping cloning language line for '{scheme}:{node.id}' as its already registered.");
                    break;
                }

                string origLangKey = string.Format(DisplayNameFormat, scheme, node.id);

                try
                {
                    if (Language.main.TryGet(origLangKey, out string origString))
                    {
                        LanguageHandler.SetLanguageLine(clonedLangKey, origString);
                    }
                    else
                    {
                        QuickLogger.Warning($"Problem cloning language line for '{scheme}:{node.id}'{Environment.NewLine}Language resource not found");
                    }
                }
                catch (Exception ex)
                {
                    QuickLogger.Error($"Error cloning language line for '{scheme}:{node.id}'{Environment.NewLine}{ex.Message}");
                }

                string origSpriteKey = string.Format(TabSpriteFormat, scheme, node.id);
                string clonedSpriteKey = string.Format(TabSpriteFormat, AioFabScheme, node.id);
                try
                {
                    var groupSprite = SpriteManager.Get(SpriteManager.Group.Category, origSpriteKey);
                    if (groupSprite != null)
                    {
                        SpriteHandler.RegisterSprite(SpriteManager.Group.Category, clonedSpriteKey, groupSprite);
                    }
                    else
                    {
                        QuickLogger.Warning($"Problem cloning sprite for '{scheme}:{node.id}'{Environment.NewLine}Sprite resource not found");
                    }
                }
                catch (Exception ex)
                {
                    QuickLogger.Error($"Error cloning sprite for '{scheme}:{node.id}'{Environment.NewLine}{ex.Message}");
                }
                break;
            }
        }

        foreach (CraftNode innerNode in node)
            CloneTabDetails(scheme, innerNode);
    }

    public static void ModifyGameObject(GameObject gObj)
    {
        if (texture != null)
        {
            // Set the custom texture
            SkinnedMeshRenderer skinnedMeshRenderer = gObj.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.material.mainTexture = texture;
        }

        // Change size
        Vector3 scale = gObj.transform.localScale;
        const float factor = 1.25f;
        gObj.transform.localScale = new Vector3(scale.x * factor, scale.y * factor, scale.z * factor);
    }

    public static RecipeData GetBlueprintRecipe()
    {
        return new RecipeData
        {
            craftAmount = 1,
            Ingredients =
            {
                new Ingredient(TechType.Titanium, 3),
                new Ingredient(TechType.ComputerChip, 2),
                new Ingredient(TechType.WiringKit, 1),
                new Ingredient(TechType.Diamond, 1),
                new Ingredient(TechType.AluminumOxide, 1),
                new Ingredient(TechType.Magnetite, 1)
            }
        };
    }

}
