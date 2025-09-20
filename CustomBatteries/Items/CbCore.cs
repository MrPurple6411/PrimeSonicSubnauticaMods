namespace CustomBatteries.Items;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common;
using CustomBatteries.API;
using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;
using static CraftData;

internal abstract class CbCore
{
    protected abstract TechType PrefabType { get; } // Should only ever be Battery, IonBattery, PowerCell or IonPowerCell
    protected abstract EquipmentType ChargerType { get; } // Should only ever be BatteryCharger or PowerCellCharger

    public TechType RequiredForUnlock { get; set; } = TechType.None;
    public bool UnlocksAtStart => this.RequiredForUnlock == TechType.None;

    public virtual RecipeData GetBlueprintRecipe()
    {
        var partsList = new List<Ingredient>();

        CreateIngredients(this.Parts, partsList);

        if (partsList.Count == 0)
            partsList.Add(new Ingredient(TechType.Titanium, 1));

        var batteryBlueprint = new RecipeData
        {
            craftAmount = 1,
            Ingredients = partsList
        };

        return batteryBlueprint;
    }

    public float PowerCapacity { get; set; }

    public string FriendlyName { get; set; }

    public string Description { get; set; }

    public string IconFileName { get; set; }

    public string PluginPackName { get; set; }

    public string PluginFolder { get; set; }

    public Sprite Sprite { get; set; }

    public IList<TechType> Parts { get; set; }

    public bool IsPatched { get; private set; }

    public bool UsingIonCellSkins { get; }

    public CBModelData CustomModelData { get; set; }

    protected Action<GameObject> EnhanceGameObject { get; set; }

    public bool AddToFabricator { get; set; } = true;

    public required PrefabInfo Info { get; set; }

    private CustomPrefab CustomPrefab { get; set; }

    protected CbCore(string classId, bool ionCellSkins)
    {
        this.ClassID = classId;
        this.UsingIonCellSkins = ionCellSkins;
    }

    protected CbCore(CbItem packItem): this(packItem.ID, packItem.CBModelData?.UseIonModelsAsBase ?? false)
    {
        this.CustomModelData = packItem.CBModelData;
        this.Sprite = packItem.CustomIcon;
        this.EnhanceGameObject = packItem.EnhanceGameObject;
        this.AddToFabricator = packItem.AddToFabricator;
    }

    public IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
    {
        var taskResult = new TaskResult<GameObject>();
        yield return CraftData.InstantiateFromPrefabAsync(this.PrefabType, taskResult);
        var obj = taskResult.Get();

        Battery battery = obj.GetComponent<Battery>();
        battery._capacity = this.PowerCapacity;
        battery.name = $"{this.ClassID}BatteryCell";

        // If "Enable batteries/powercells placement" feature from Decorations mod is ON.
        if (CbDatabase.PlaceBatteriesFeatureEnabled && TechData.GetEquipmentType(this.Info.TechType) != EquipmentType.Hand)
        {
            CraftDataHandler.SetEquipmentType(this.Info.TechType, EquipmentType.Hand); // Set equipment type to Hand.
            CraftDataHandler.SetQuickSlotType(this.Info.TechType, QuickSlotType.Selectable); // We can select the item.
        }

        SkyApplier skyApplier = obj.EnsureComponent<SkyApplier>();
        skyApplier.renderers = obj.GetComponentsInChildren<Renderer>(true);
        skyApplier.anchorSky = Skies.Auto;

        if (CustomModelData != null)
        {
            foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>(true))
            {
                if (CustomModelData.CustomTexture != null)
                    renderer.material.SetTexture(ShaderPropertyID._MainTex, this.CustomModelData.CustomTexture);

                if (CustomModelData.CustomNormalMap != null)
                    renderer.material.SetTexture(ShaderPropertyID._BumpMap, this.CustomModelData.CustomNormalMap);

                if (CustomModelData.CustomSpecMap != null)
                    renderer.material.SetTexture(ShaderPropertyID._SpecTex, this.CustomModelData.CustomSpecMap);

                if (CustomModelData.CustomIllumMap != null)
                {
                    renderer.material.SetTexture(ShaderPropertyID._Illum, this.CustomModelData.CustomIllumMap);
                    renderer.material.SetFloat(ShaderPropertyID._GlowStrength, this.CustomModelData.CustomIllumStrength);
                    renderer.material.SetFloat(ShaderPropertyID._GlowStrengthNight, this.CustomModelData.CustomIllumStrength);
                }
            }
        }

        this.EnhanceGameObject?.Invoke(obj);

        gameObject.Set(obj);
    }

    protected void CreateIngredients(IEnumerable<TechType> parts, List<Ingredient> partsList)
    {
        if (parts == null)
            return;

        foreach (TechType part in parts)
        {
            if (part == TechType.None)
            {
                QuickLogger.Warning($"Parts list for '{this.ClassID}' contained an unidentified TechType");
                continue;
            }

            Ingredient priorIngredient = partsList.Find(i => i.techType == part);

            if (priorIngredient != null)
                priorIngredient._amount++;
            else
                partsList.Add(new Ingredient(part, 1));
        }
    }

    protected abstract void AddToList();

    protected abstract string[] StepsToFabricatorTab { get; }

    public string ClassID { get; init; }

    public void Patch()
    {
        if (this.IsPatched)
            return;

        CustomPrefab = new CustomPrefab(Info);
        CustomPrefab.SetGameObject(GetGameObjectAsync);

        if (this.CustomModelData != null)
        {
            if (this.ChargerType == EquipmentType.BatteryCharger && !CbDatabase.BatteryModels.ContainsKey(Info.TechType))
            {
                CbDatabase.BatteryModels.Add(Info.TechType, this.CustomModelData);
            }
            else if (this.ChargerType == EquipmentType.PowerCellCharger && !CbDatabase.PowerCellModels.ContainsKey(Info.TechType))
            {
                CbDatabase.PowerCellModels.Add(Info.TechType, this.CustomModelData);
            }
        }

        if (!this.UnlocksAtStart)
            KnownTechHandler.SetAnalysisTechEntry(this.RequiredForUnlock, new TechType[] { Info.TechType });

        if (this.Sprite == null)
        {
            string imageFilePath = null;

            if (this.PluginFolder != null && this.IconFileName != null)
                imageFilePath = IOUtilities.Combine(CbDatabase.ExecutingFolder, this.PluginFolder, this.IconFileName);

            if (imageFilePath != null && File.Exists(imageFilePath))
                this.Sprite = ImageUtils.LoadSpriteFromFile(imageFilePath);
            else
            {
                QuickLogger.Warning($"Did not find a matching image file at {imageFilePath} or in {nameof(CbBattery.CustomIcon)}.{Environment.NewLine}Using default sprite instead.");
                this.Sprite = SpriteManager.Get(this.PrefabType);
            }
        }

        SpriteHandler.RegisterSprite(Info.TechType, this.Sprite);

        CraftDataHandler.SetRecipeData(Info.TechType, GetBlueprintRecipe());

        CraftDataHandler.AddToGroup(TechGroup.Resources, TechCategory.Electronics, Info.TechType);

        CraftDataHandler.SetEquipmentType(Info.TechType, this.ChargerType);

        if (this.AddToFabricator)
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, Info.TechType, this.StepsToFabricatorTab);

        CustomPrefab.Register();

        AddToList();

        this.IsPatched = true;
    }
}
