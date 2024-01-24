namespace MoreCyclopsUpgrades.API.Upgrades;

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

/// <summary>
/// Extends the <see cref="Craftable"/> class with handling and defaults specific for Cyclops upgrade modules.
/// </summary>
/// <seealso cref="Craftable" />
public abstract class CyclopsUpgrade
{

    public virtual string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

    /// <summary>
    /// Gets the type of equipment slot this item can fit into.
    /// </summary>
    public EquipmentType EquipmentType => EquipmentType.CyclopsModule;

    /// <summary>
    /// Overriden to ensure this item appearas within the <see cref="TechGroup.Cyclops"/> group in the PDA blurprints menu.
    /// </summary>
    public TechGroup GroupForPDA =>
#if SUBNAUTICA
        TechGroup.Cyclops;
#elif BELOWZERO
        TechGroup.VehicleUpgrades;
#endif

    /// <summary>
    /// Overrides to ensure this item appears within the <see cref="TechCategory.CyclopsUpgrades"/> category in the PDA blueprints menu.
    /// </summary>
    public TechCategory CategoryForPDA =>
#if SUBNAUTICA
        TechCategory.CyclopsUpgrades;
#elif BELOWZERO
        TechCategory.VehicleUpgrades;
#endif

    /// <summary>
    /// Gets the prefab template used to clone new instances of this upgrade module.<para/>
    /// Defaults to <see cref="TechType.CyclopsThermalReactorModule"/> which is enough for most any Cyclops upgrade module.
    /// </summary>
    /// <value>
    /// The prefab template.
    /// </value>
    protected virtual TechType PrefabTemplate { get; } = TechType.CyclopsThermalReactorModule;

    /// <summary>
    /// Overriden to set to have the <see cref="TechType.Cyclops" /> be required before this upgrade module can be unlocked.
    /// If not overriden, it this item will be unlocked from the start of the game.
    /// </summary>
    public virtual TechType RequiredForUnlock => TechType.Cyclops; // Default can be overridden by child classes

    /// <summary>
    /// Override this to set which other module in the PDA this upgrade module should be sorted after.
    /// </summary>
    public virtual TechType SortAfter => TechType.None;

    public virtual CraftTree.Type FabricatorType { get; } = CraftTree.Type.CyclopsFabricator;
    public virtual string[] StepsToFabricatorTab { get; } = new[] { "CyclopsMenu" };

    public virtual float CraftingTime => 0f;

    public PrefabInfo Info { get; private init; }
    public TechType TechType => Info.TechType;
    public CustomPrefab CustomPrefab { get; private init; }
    public bool IsPatched { get; internal set; }

    public event Action OnStartedPatching;

    public event Action OnFinishedPatching;

    /// <summary>
    /// Initializes a new instance of the <seealso cref="Craftable"/> <see cref="CyclopsUpgrade"/> class.<para/>
    /// Any item created with this class with automatically be equipable in the Cyclops.
    /// </summary>
    /// <param name="classId">The main internal identifier for this item. Your item's <see cref="T:TechType" /> will be created using this name.</param>
    /// <param name="friendlyName">The name displayed in-game for this item whether in the open world or in the inventory.</param>
    /// <param name="description">The description for this item; Typically seen in the PDA, inventory, or crafting screens.</param>
    protected CyclopsUpgrade(string classId, string friendlyName, string description)
    {
        this.Info = PrefabInfo.WithTechType(classId: classId, displayName: friendlyName, description: description, language: "English", unlockAtStart: false);

        CustomPrefab = new CustomPrefab(this.Info);

    }

    public void Patch()
    {
        if (IsPatched)
            return;

        OnStartedPatching?.Invoke();

        ScanningGadget scanningGadget;
        if (this.SortAfter != TechType.None && CraftData.GetBuilderIndex(SortAfter, out var group, out var category, out _) && group == GroupForPDA && category == CategoryForPDA)
            scanningGadget = CustomPrefab.SetPdaGroupCategoryAfter(this.GroupForPDA, this.CategoryForPDA, this.SortAfter);
        else
            scanningGadget = CustomPrefab.SetPdaGroupCategory(this.GroupForPDA, this.CategoryForPDA);

        scanningGadget.RequiredForUnlock = this.RequiredForUnlock;

        var iconPath = Path.Combine(this.AssetsFolder, $"{this.Info.ClassID}.png");
        if (File.Exists(iconPath))
        {
            Texture2D spriteTexture = ImageUtils.LoadTextureFromFile(iconPath);
            this.Info.WithIcon(ImageUtils.LoadSpriteFromTexture(spriteTexture));
            scanningGadget.WithAnalysisTech(Sprite.Create(spriteTexture, new Rect(0f, 0f, spriteTexture.width, spriteTexture.height), new Vector2(0.5f, 0.5f)), null, null);
        }
        else
        {
            this.Info.WithIcon(SpriteManager.Get(PrefabTemplate));
            scanningGadget.WithAnalysisTech(null, null, null);
        }

        CustomPrefab.SetRecipe(GetBlueprintRecipe())
            .WithFabricatorType(FabricatorType)
            .WithStepsToFabricatorTab(StepsToFabricatorTab)
            .WithCraftingTime(CraftingTime);

        CustomPrefab.SetEquipment(EquipmentType);
        CustomPrefab.SetGameObject(GetGameObjectAsync);

        CustomPrefab.Register();
        OnFinishedPatching?.Invoke();
        IsPatched = true;
    }

    protected virtual RecipeData GetBlueprintRecipe() => CraftDataHandler.GetRecipeData(PrefabTemplate);

    /// <summary>
    /// Gets the prefab game object. Set up your prefab components here.<para/>
    /// A default implementation is already provided which creates the new item by modifying a clone of the item defined in <see cref="PrefabTemplate"/>.
    /// </summary>
    /// <returns>
    /// The game object to be instantiated into a new in-game entity.
    /// </returns>
    public virtual IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
    {
        var task = new TaskResult<GameObject>();
        yield return CraftData.InstantiateFromPrefabAsync(this.PrefabTemplate, task);
        gameObject.Set(task.Get());
    }

    /// <summary>
    /// A utility method that spawns a cyclops upgrade module by TechType ID.
    /// </summary>
    /// <param name="techTypeID">The tech type ID.</param>
    /// <returns>A new <see cref="InventoryItem"/> that wraps up a <see cref="Pickupable"/> game object.</returns>
    public static InventoryItem SpawnCyclopsModule(TechType techTypeID)
    {
        try
        {
            TaskResult<GameObject> task = new TaskResult<GameObject>();
            var coroutine = CraftData.InstantiateFromPrefabAsync(techTypeID, task);

            // DON'T EVER DO THIS! This is a hack to force the coroutine to run to completion.
            // TODO: REWRITE THIS TO BE NON-BLOCKING!!
            while (coroutine.MoveNext())
            { }

            GameObject prefab = task.Get();

            if (prefab == null)
                return null;

            var gameObject = GameObject.Instantiate(prefab);

            Pickupable pickupable = gameObject.GetComponent<Pickupable>();
            pickupable.Pickup(false);
            return new InventoryItem(pickupable);
        }
        catch
        {
            return null;
        }
    }
}
