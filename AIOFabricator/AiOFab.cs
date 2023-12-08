namespace AIOFabricator;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Common;
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
    private const string FabricatorScheme = "Fabricator";
    private const string WorkBenchScheme = "Workbench";
    private const string SeamothUpgradesScheme = "SeamothUpgrades";
    private const string MapRoomScheme = "MapRoom";
#if SUBNAUTICA
    private const string CyclopsFabScheme = "CyclopsFabricator";
#elif BELOWZERO
    private const string SeaTruckFabScheme = "SeaTruckFabricator";
#endif

    public static CraftTree.Type TreeTypeID { get; private set; }
    public static ModCraftTreeRoot Root { get; private set; }

    private static CraftTree craftTree;
    private static Texture2D texture;
    private static Texture2D spriteTexture;

    internal static void CreateAndRegister()
    {
        LoadImageFiles();

        var Info = PrefabInfo.WithTechType(AioFabScheme, "All-In-One Fabricator",
               "Multi-fuction fabricator capable of synthesizing most blueprints.", "English", false)
            .WithIcon(spriteTexture != null ? ImageUtils.LoadSpriteFromTexture(spriteTexture) : SpriteManager.Get(TechType.Fabricator));

        var prefab = new CustomPrefab(Info);

        if(GetBuilderIndex(TechType.Fabricator, out var group, out var category, out _))
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
        prefab.AddOnRegister(RegisterCraftTreeBasics);

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

    private static void RegisterCraftTreeBasics()
    {
        RegisterTopLevelVanillaTab(FabricatorScheme, "Fabricator", TechType.Fabricator);
        RegisterTopLevelVanillaTab(WorkBenchScheme, "Modification Station", TechType.Workbench);
        RegisterTopLevelVanillaTab(SeamothUpgradesScheme, "Vehicle Upgrades", TechType.BaseUpgradeConsole);
        RegisterTopLevelVanillaTab(MapRoomScheme, "Scanner Room", TechType.BaseMapRoom);
#if SUBNAUTICA
        RegisterTopLevelVanillaTab(CyclopsFabScheme, "Cyclops Upgrades", TechType.Cyclops);
#endif

        Root.CraftTreeCreation = CreateCraftingTree;
    }

    private static CraftTree CreateCraftingTree()
    {
        if (craftTree == null)
        {
            List<CraftNode> craftNodes = new List<CraftNode>();

            CraftNode fab = CraftTree.FabricatorScheme();
            CloneTabDetails(FabricatorScheme, fab);
            craftNodes.Add(fab);

            CraftNode wb = CraftTree.WorkbenchScheme();
            CloneTabDetails(WorkBenchScheme, wb);
            craftNodes.Add(wb);

            CraftNode su = CraftTree.SeamothUpgradesScheme();
            CloneTabDetails(SeamothUpgradesScheme, su);
            craftNodes.Add(su);

            CraftNode map = CraftTree.MapRoomSheme();
            CloneTabDetails(MapRoomScheme, map);
            craftNodes.Add(map);
#if SUBNAUTICA
            CraftNode cy = CraftTree.CyclopsFabricatorScheme();
            CloneTabDetails(CyclopsFabScheme, cy);
            craftNodes.Add(cy);
#endif

            CraftNode aioRoot = new CraftNode("Root").AddNode(craftNodes.ToArray());

            Type smlCTPatcher = typeof(CraftTreeHandler).Assembly.GetType("Nautilus.Patchers.CraftTreePatcher");
            var customTrees = (Dictionary<CraftTree.Type, ModCraftTreeRoot>)smlCTPatcher.GetField("CustomTrees", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            foreach (KeyValuePair<CraftTree.Type, ModCraftTreeRoot> entry in customTrees)
            {
                if (entry.Key == TreeTypeID)
                    continue;

                CraftTree tree = entry.Value.CraftTreeCreation.Invoke();
                CraftNode root = tree.nodes;
                string scheme = entry.Key.ToString();

                CloneTabDetails(scheme, root);
                CloneTopLevelModTab(scheme);
                aioRoot.AddNode(root);
            }

            craftTree = new CraftTree(AioFabScheme, aioRoot);
        }

        return craftTree;
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

    private static void RegisterTopLevelVanillaTab(string scheme, string tabDisplayName, TechType tabIconId)
    {
        SpriteHandler.RegisterSprite(SpriteManager.Group.Category, string.Format(TabSpriteFormat, AioFabScheme, scheme), SpriteManager.Get(tabIconId));
        LanguageHandler.SetLanguageLine(string.Format(DisplayNameFormat, AioFabScheme, scheme), tabDisplayName);
    }

    private static void CloneTopLevelModTab(string scheme)
    {
        string clonedLangKey = string.Format(DisplayNameFormat, AioFabScheme, scheme);

        if (Language.main.TryGet(scheme, out string origString))
        {
            LanguageHandler.SetLanguageLine(clonedLangKey, origString);
        }
        else
        {
            QuickLogger.Warning($"Problem cloning language line for '{scheme}:root'{Environment.NewLine}Language resource not found");
        }

        string clonedSpriteKey = string.Format(TabSpriteFormat, AioFabScheme, scheme);

        if (TechTypeExtensions.FromString(scheme, out TechType techType, true))
        {
            var rootSprite = SpriteManager.Get(techType);
            SpriteHandler.RegisterSprite(SpriteManager.Group.Category, clonedSpriteKey, rootSprite);
        }
        else
        {
            QuickLogger.Warning($"Problem cloning sprite for '{scheme}:root'{Environment.NewLine}Sprite resource not found");
        }
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
}
