namespace MoreCyclopsUpgrades.API.Buildables;

// This partial class file contains all members of AuxiliaryUpgradeConsole intended for public/external use

/// <summary>
/// The core functionality of an Auxiliary Upgrade Console.<para/>
/// Handles basic player interaction, save data, and connecting with the Cyclops sub.
/// </summary>
/// <seealso cref="HandTarget" />
/// <seealso cref="IHandTarget" />
/// <seealso cref="IProtoEventListener" />
/// <seealso cref="ICyclopsBuildable" /> 
public abstract partial class AuxiliaryUpgradeConsole : UpgradeConsole, ICyclopsBuildable
{
    /// <summary>
    /// Invoked after <see cref="OnEquip(string, InventoryItem)"/> has finished handling the added item.
    /// </summary>
    public virtual void OnSlotEquipped(string slot, InventoryItem item) { }

    /// <summary>
    /// Invoked after <see cref="OnUnequip(string, InventoryItem)"/> has finished handling the removed item.
    /// </summary>
    public virtual void OnSlotUnequipped(string slot, InventoryItem item) { }

    /// <summary>
    /// Gets a value indicating whether this buildable is connected to the Cyclops.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this buildable is connected to cyclops; otherwise, <c>false</c>.
    /// </value>
    /// <see cref="ICyclopsBuildable"/>
    /// <seealso cref="BuildableManager{BuildableMono}.ConnectWithManager(BuildableMono)" />
    public bool IsConnectedToCyclops => ParentCyclops != null && _upgradeManager != null;

}
