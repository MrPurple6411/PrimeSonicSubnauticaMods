namespace BetterBioReactor;

using Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ItemsContainer;

internal class BioReactorController
{
    private static readonly Dictionary<BaseBioReactor, BioReactorController> LookupReactorController = new();
    internal static BioReactorController GetMiniReactor(ref BaseBioReactor bioReactor)
    {
        if (LookupReactorController.TryGetValue(bioReactor, out BioReactorController existingBioMini))
            return existingBioMini;

        var createdBioController = new BioReactorController(ref bioReactor);
        LookupReactorController.Add(bioReactor, createdBioController);

        QuickLogger.Warning("BioReactorController Connected");

        return createdBioController;
    }

    internal static bool PdaIsOpen = false;
    internal static BioReactorController OpenInPda = null;

    private const float TextDelayInterval = 2f;
    private float textDelay = TextDelayInterval;
    private float NumberOfContainerSlots => BioReactor.container.sizeX * BioReactor.container.sizeY;

    private readonly HashSet<BioEnergy> MaterialsProcessing = new();
    private readonly HashSet<BioEnergy> FullyConsumed = new();

    public int MaxPower = -1;
    public string MaxPowerText => $"{MaxPower}{(BioReactor.producingPower ? "+" : "")}";
    public int CurrentPower => Mathf.RoundToInt(BioReactor._powerSource.GetPower());

    public readonly BaseBioReactor BioReactor;
    private BaseBioReactorGeometry fallbackGeometry = null;

    // Careful, this map only exists while the PDA screen is open
    public Dictionary<InventoryItem, uGUI_ItemIcon> InventoryMapping { get; private set; }

    public BioReactorController(ref BaseBioReactor bioReactor)
    {
        BioReactor = bioReactor;

        PrefabIdentifier prefabIdentifier = bioReactor.GetComponentInParent<PrefabIdentifier>() ?? bioReactor.GetComponent<PrefabIdentifier>();

        string id = prefabIdentifier.Id;

        QuickLogger.Warning($"BioReactorController PrefabIdentifier: {id}");
    }

    public void Start()
    {
        QuickLogger.Warning("BioReactorController starting");
        (BioReactor.container as IItemsContainer).onAddItem += OnAddItem;
        BioReactor._powerRelay = PowerSource.FindRelay(BioReactor.transform);

        MaxPower = Mathf.RoundToInt(BioReactor._powerSource.GetMaxPower());
        RestoreItemsFromSaveData();
        QuickLogger.Warning("BioReactorController started");
    }

    #region Player interaction

    // Completely replaces the original OnHover method in the BaseBioReactor
    internal void OnHover()
    {
        HandReticle main = HandReticle.main;

        string text1 = Language.main.GetFormat("UseBaseBioReactor", this.CurrentPower, this.MaxPowerText);
        main.SetText(HandReticle.TextType.Hand, text1, false, GameInput.Button.LeftHand);
        main.SetText(HandReticle.TextType.HandSubscript, "Tooltip_UseBaseBioReactor", true);
        main.SetIcon(HandReticle.IconType.Hand, 1f);
    }

    // Completely replaces the original OnUse method in the BaseBioReactor
    internal void OnPdaOpen(BaseBioReactorGeometry model)
    {
        fallbackGeometry = model;

        PdaIsOpen = true;
        OpenInPda = this;

        PDA pda = Player.main.GetPDA();
        Inventory.main.SetUsedStorage(BioReactor.container, false);
        pda.Open(PDATab.Inventory, model.storagePivot, new PDA.OnClose(OnPdaClose));
    }

    internal void OnPdaClose(PDA pda)
    {
        this.InventoryMapping = null;

        foreach (BioEnergy item in MaterialsProcessing)
        {
            GameObject.Destroy(item.DisplayText.gameObject);
            item.DisplayText = null;
        }

        PdaIsOpen = false;
        OpenInPda = null;

        (BioReactor.container as IItemsContainer).onAddItem -= OnAddItemLate;
    }

    private void OnAddItem(InventoryItem item)
    {
        if (BaseBioReactor.charge.TryGetValue(item.item.GetTechType(), out float bioEnergyValue) && bioEnergyValue > 0f)
        {
            BioEnergy bioEnergy = item.item.gameObject.EnsureComponent<BioEnergy>();
            bioEnergy.Pickupable = item.item;
            if (bioEnergy.RemainingEnergy == 0f)
                bioEnergy.RemainingEnergy = bioEnergy.MaxEnergy;
            MaterialsProcessing.Add(bioEnergy);
        }
        else
        {
            QuickLogger.Error($"Item {item.item.GetTechType()} is not a valid bio energy source", true);
            GameObject.Destroy(item.item);
        }
    }

    private void OnAddItemLate(InventoryItem item)
    {
        if (this.InventoryMapping is null)
            return;

        if (this.InventoryMapping.TryGetValue(item, out uGUI_ItemIcon icon))
        {
            BioEnergy bioEnergy = item.item.gameObject.EnsureComponent<BioEnergy>();
            bioEnergy.Pickupable = item.item;
            bioEnergy.AddDisplayText(icon);
        }
    }

    #endregion

    // This method completely replaces the original ProducePower method in the BaseBioReactor
    internal float ProducePower(float requested)
    {
        float powerProduced = 0f;

        while (MaterialsProcessing.Count > 0 && powerProduced < requested)
        {
            if (requested > 0f && // More than zero energy being produced per item per time delta
                MaterialsProcessing.Count > 0) // There should be materials in the reactor to process
            {
                float chargePerSecondPerItem = requested / NumberOfContainerSlots;

                foreach (BioEnergy material in MaterialsProcessing)
                {
                    float availablePowerPerItem = Mathf.Min(material.RemainingEnergy, material.Size * chargePerSecondPerItem);

                    material.RemainingEnergy -= availablePowerPerItem;
                    powerProduced += availablePowerPerItem;

                    if (material.FullyConsumed)
                        FullyConsumed.Add(material);
                    else if (PdaIsOpen)
                        material.UpdateInventoryText();
                }
            }

            if (FullyConsumed.Count > 0)
            {
                foreach (BioEnergy material in FullyConsumed)
                {
                    MaterialsProcessing.Remove(material);
                    BioReactor.container.RemoveItem(material.Pickupable, true);
                    GameObject.Destroy(material.Pickupable.gameObject);
                }

                FullyConsumed.Clear();
            }
        }
        return powerProduced;
    }

    internal void UpdateDisplayText()
    {
        if (Time.time < textDelay)
            return; // Slow down the text update

        textDelay = Time.time + TextDelayInterval;

        if (MaterialsProcessing.Count > 0 || this.CurrentPower > 0)
        {
            TMPro.TextMeshProUGUI displayText = (BioReactor.GetModel() ?? fallbackGeometry)?.text;
            if (displayText != null)
            {
                string maxPowerText = this.MaxPowerText;
                string currentPowerString = this.CurrentPower.ToString().PadLeft(maxPowerText.Length, ' ');
                displayText.text = $"<color=#00ff00>{Language.main.Get("BaseBioReactorActive")}\n{currentPowerString}/{maxPowerText}</color>";
            }
        }
    }

    #region Save data handling

    public Dictionary<string, float> SaveData;

    private void RestoreItemsFromSaveData()
    {
        if (SaveData is null)
        {
            try
            {
                var saveFolder = Path.Combine(SaveLoadManager.GetTemporarySavePath(), "BetterBioReactor");
                if (!Directory.Exists(saveFolder))
                    Directory.CreateDirectory(saveFolder);

                var uniqueIdentifier = BioReactor.GetComponentInParent<UniqueIdentifier>();
                var savePath = Path.Combine(saveFolder, $"BBRSaveData_{uniqueIdentifier.Id}.json");
                if (File.Exists(savePath))
                    SaveData = JsonConvert.DeserializeObject<Dictionary<string, float>>(File.ReadAllText(savePath));
                else
                    SaveData = new Dictionary<string, float>();

            }
            catch (System.Exception e)
            {
                QuickLogger.Error("Failed to load BetterBioreactor Save Data!!!!!", true);
                QuickLogger.Error(e.Message);
                SaveData = new Dictionary<string, float>();
            }

            
        }

        foreach (var pair in BioReactor.container._items)
        {
            TechType techType = pair.Key;
            ItemGroup itemGroup = pair.Value;

            foreach (InventoryItem item in itemGroup.items)
            {
                BioEnergy bioEnergy = item.item.gameObject.EnsureComponent<BioEnergy>();
                bioEnergy.Pickupable = item.item;

                UniqueIdentifier uniqueIdentifier = item.item.GetComponent<UniqueIdentifier>();
                if (SaveData.TryGetValue(uniqueIdentifier.Id, out float remainingEnergy))
                    bioEnergy.RemainingEnergy = remainingEnergy;
                else
                    bioEnergy.RemainingEnergy = bioEnergy.MaxEnergy;

                SaveData[uniqueIdentifier.Id] = bioEnergy.RemainingEnergy;
                MaterialsProcessing.Add(bioEnergy);
            }
        }

        QuickLogger.Debug("Original items restored");
    }

    #endregion

    public void ConnectToInventory(Dictionary<InventoryItem, uGUI_ItemIcon> lookup)
    {
        this.InventoryMapping = lookup;

        (BioReactor.container as IItemsContainer).onAddItem += OnAddItemLate;
        foreach (KeyValuePair<InventoryItem, uGUI_ItemIcon> pair in lookup)
        {
            InventoryItem item = pair.Key;
            uGUI_ItemIcon icon = pair.Value;

            BioEnergy bioEnergy = item.item.gameObject.EnsureComponent<BioEnergy>();
            bioEnergy.Pickupable = item.item;
            bioEnergy.AddDisplayText(icon);
        }
    }

    internal void OnProtoSerializeObjectTree()
    {
        foreach (BioEnergy bioEnergy in MaterialsProcessing)
        {
            UniqueIdentifier uniqueIdentifier = bioEnergy.Pickupable.GetComponent<UniqueIdentifier>();
            SaveData[uniqueIdentifier.Id] = bioEnergy.RemainingEnergy;
        }

        try
        {
            var saveFolder = Path.Combine(SaveLoadManager.GetTemporarySavePath(), "BetterBioReactor");
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            var uniqueIdentifier = BioReactor.GetComponentInParent<UniqueIdentifier>();
            var savePath = Path.Combine(saveFolder, $"BBRSaveData_{uniqueIdentifier.Id}.json");

            File.WriteAllText(savePath, JsonConvert.SerializeObject(SaveData, Formatting.Indented));

            QuickLogger.Debug($"Saved BetterBioreactor Save Data to {savePath}", true);
        }
        catch (System.Exception e)
        {
            QuickLogger.Error("Failed to save BetterBioreactor Save Data!!!!!", true);
            QuickLogger.Error(e.Message);
        }
    }
}