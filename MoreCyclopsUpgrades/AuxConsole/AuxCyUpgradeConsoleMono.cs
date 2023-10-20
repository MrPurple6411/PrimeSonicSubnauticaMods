﻿namespace MoreCyclopsUpgrades.AuxConsole;

using MoreCyclopsUpgrades.API.Buildables;
using UnityEngine;

internal class AuxCyUpgradeConsoleMono : AuxiliaryUpgradeConsole
{
    private ModuleIconDisplay IconDisplay;

    public override void OnSlotEquipped(string slot, InventoryItem item)
    {
        if (IconDisplay == null)
            AddModuleSpriteHandlers();

        IconDisplay.EnableIcon(slot, this.modules.GetTechTypeInSlot(slot));
    }

    public override void OnSlotUnequipped(string slot, InventoryItem item)
    {
        if (IconDisplay == null)
            AddModuleSpriteHandlers();

        IconDisplay.DisableIcon(slot);
    }

    private void AddModuleSpriteHandlers()
    {
        const float topRowY = 1.15f;//-0.109f;
        const float botRowY = 1.075f;//-0.239f;

        const float leftColX = 0.15f;//0.159f;
        const float middColX = 0f;//0f;
        const float rigtColX = -0.15f;//-0.152f;

        const float topRowZ = 0.12f;// 1.146f;
        const float botRowZ = 0.270f;//1.06f;

        var rotation = Quaternion.Euler(62.5f, 180, 0);

        Canvas display1 = IconCreator.CreateModuleDisplay(this.gameObject, new Vector3(rigtColX, botRowY, botRowZ), rotation, modules.GetTechTypeInSlot("Module1"));
        Canvas display2 = IconCreator.CreateModuleDisplay(this.gameObject, new Vector3(middColX, botRowY, botRowZ), rotation, modules.GetTechTypeInSlot("Module2"));
        Canvas display3 = IconCreator.CreateModuleDisplay(this.gameObject, new Vector3(leftColX, botRowY, botRowZ), rotation, modules.GetTechTypeInSlot("Module3"));
        Canvas display4 = IconCreator.CreateModuleDisplay(this.gameObject, new Vector3(rigtColX, topRowY, topRowZ), rotation, modules.GetTechTypeInSlot("Module4"));
        Canvas display5 = IconCreator.CreateModuleDisplay(this.gameObject, new Vector3(middColX, topRowY, topRowZ), rotation, modules.GetTechTypeInSlot("Module5"));
        Canvas display6 = IconCreator.CreateModuleDisplay(this.gameObject, new Vector3(leftColX, topRowY, topRowZ), rotation, modules.GetTechTypeInSlot("Module6"));

        IconDisplay = new ModuleIconDisplay(display1, display2, display3, display4, display5, display6);
    }
}
