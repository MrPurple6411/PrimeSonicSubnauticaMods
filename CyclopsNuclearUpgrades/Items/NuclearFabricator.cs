namespace CyclopsNuclearUpgrades;

using System.IO;
using System.Reflection;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;
using static CraftData;
using static Nautilus.Assets.PrefabTemplates.FabricatorTemplate;

internal class NuclearFabricator
{
    public static PrefabInfo Info { get; private set; }
    public static CustomPrefab CustomPrefab { get; private set; }
    public static FabricatorGadget Fabricator { get; private set; }

    public const string ClassId = "NuclearFabricator";
    public const string DisplayName = "Nuclear Fabricator";
    public const string Description = "A specialized fabricator for safe handling of radioactive energy sources.";

    public static Texture2D customTexture;
    private static CraftTree.Type _craftTreeType;

    private static void ModifyPrefab(GameObject gObj)
    {
        // Set the custom texture
        if (customTexture != null)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = gObj.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.material.mainTexture = customTexture;
        }

        // Change size
        Vector3 scale = gObj.transform.localScale;
        const float factor = 0.90f;
        gObj.transform.localScale = new Vector3(scale.x * factor, scale.y * factor, scale.z * factor);
        gObj.SetActive(false);
    }

    private static RecipeData GetBlueprintRecipe()
    {
        return new RecipeData()
        {
            craftAmount = 1,
            Ingredients =
            {
                new Ingredient(TechType.Titanium, 2),
                new Ingredient(TechType.ComputerChip, 1),
                new Ingredient(TechType.Magnetite, 1),
                new Ingredient(TechType.Lead, 2),
            }
        };
    }

    internal static void CreateAndRegister()
    {
        Info = PrefabInfo.WithTechType(ClassId, DisplayName, Description, "English", false);

        CraftDataHandler.SetRecipeData(Info.TechType, GetBlueprintRecipe());

        // Load the custom texture
        string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string folderPath = Path.Combine(executingLocation, "Assets");

        string textureLocation = Path.Combine(folderPath, "NuclearFabricatorT.png");
        if (File.Exists(textureLocation))
        customTexture = ImageUtils.LoadTextureFromFile(textureLocation);

        // Create the prefab
        CustomPrefab = new CustomPrefab(Info);
        var scanningGadget = CustomPrefab.SetUnlock(TechType.BaseNuclearReactor)
            .WithPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);

        string spriteLocation = Path.Combine(folderPath, "NuclearFabricator.png");
        if (File.Exists(spriteLocation))
        {
            var spriteTexture = ImageUtils.LoadTextureFromFile(spriteLocation);
            Info.WithIcon(ImageUtils.LoadSpriteFromTexture(spriteTexture));
            scanningGadget.WithAnalysisTech(Sprite.Create(spriteTexture, new Rect(0f, 0f, spriteTexture.width, spriteTexture.height), new Vector2(0.5f, 0.5f)), null, Description);
        }
        else
        {
            Info.WithIcon(SpriteManager.Get(TechType.Fabricator));
            scanningGadget.WithAnalysisTech(null, null, Description);
        }

        Fabricator = CustomPrefab.CreateFabricator(out _craftTreeType);

        // Set the prefab's GameObject
        CustomPrefab.SetGameObject(new FabricatorTemplate(Info, _craftTreeType)
        {
            FabricatorModel = Model.Fabricator,
            ModifyPrefab = ModifyPrefab,
        });

        CustomPrefab.Register();
    }
}
