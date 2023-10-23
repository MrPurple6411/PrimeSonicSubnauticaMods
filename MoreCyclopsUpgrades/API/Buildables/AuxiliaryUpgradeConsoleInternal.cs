namespace MoreCyclopsUpgrades.API.Buildables;

using System.Collections;
using Common;
using MoreCyclopsUpgrades.Managers;
using ProtoBuf;
using UnityEngine;

// This partial class file contains all members of AuxiliaryUpgradeConsole intended for internal use only
[ProtoContract]
public abstract partial class AuxiliaryUpgradeConsole
{
    #region // Private/Internal members

    internal SubRoot ParentCyclops => _parentCyclops ??= GetComponentInParent<SubRoot>();
    internal Constructable Buildable => _buildable ??= GetComponentInParent<Constructable>();

    private UpgradeManager _upgradeManager;
    private Constructable _buildable;
    private SubRoot _parentCyclops;
    #endregion

    #region // Initialization (for internal use only)

    public override void Awake()
    {
        base.Awake();

        modules.onEquip += OnSlotEquipped;
        modules.onUnequip += OnSlotUnequipped;
    }

    internal virtual IEnumerator Start()
    {
        if (ParentCyclops == null)
        {
            QuickLogger.Debug("CyUpgradeConsoleMono: Could not find Cyclops during Start. Attempting external syncronize.");
            for (int i = 0; i < CyclopsManager.Managers.Count; i++)
                CyclopsManager.Managers[i].Upgrade.SyncBuildables();
        }
        else if (!ParentCyclops.isCyclops)
        {
            QuickLogger.Error("DEVELOPER WARNING: The AuxiliaryUpgradeConsole is inside a base and not a Cyclops sub. This should NOT be allowed.");
            if (Buildable != null)
            {
                TaskResult<bool> result = new TaskResult<bool>();
                TaskResult<string> resultReason = new TaskResult<string>();
                do
                {
                    yield return Buildable.DeconstructAsync(result, resultReason);
                    string text = resultReason.Get();
                    if (!string.IsNullOrEmpty(text))
                    {
                        yield return new WaitForSecondsRealtime(1f);
                    }
                }
                while (Buildable.constructedAmount > 0f);
            }
        }
        else
        {
            QuickLogger.Debug("CyUpgradeConsoleMono: Parent cyclops found!");
            ConnectToCyclops(_upgradeManager);
        }
    }

    internal void ConnectToCyclops(UpgradeManager manager = null)
    {
        var parentCyclops = ParentCyclops;
        _upgradeManager = manager ?? CyclopsManager.GetManager(ref parentCyclops)?.Upgrade;

        if (_upgradeManager != null)
        {
            _upgradeManager.AddBuildable(this);

            Equipment console = this.modules;
            _upgradeManager.AttachEquipmentEvents(ref console);
            QuickLogger.Debug("Auxiliary Upgrade Console has been connected", true);
        }
        else
        {
            QuickLogger.Error("There was a problem connecting with the parent cyclops.");
        }
    }

    #endregion

    internal virtual void OnDestroy()
    {
        _upgradeManager?.RemoveBuildable(this);
        _parentCyclops = null;
        _upgradeManager = null;
    }
}
