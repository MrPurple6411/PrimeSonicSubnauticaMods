#if !UNITY_EDITOR && (SUBNAUTICA || BELOWZERO)
namespace CustomCraft3.Serialization.Entries
{
    using System;
    using System.Linq;
    using Common;
    using CustomCraft3.Interfaces.InternalUse;
    using CustomCraft3.Serialization;

    internal partial class CfCustomCraftingTab : CustomCraftingTab, ICustomFabricatorEntry
    {
        public CraftTreePath GetCraftTreePath()
        {
            return new CraftTreePath(this.ParentTabPath, this.TabID);
        }

        protected override bool ValidFabricator()
        {
            if (!this.ParentTabPath.StartsWith(this.ParentFabricator.ItemID))
            {
                QuickLogger.Warning($"Inner {this.Key} for {this.ParentFabricator.Key} from {this.Origin} appears to have a {ParentTabPathKey} for another fabricator '{this.ParentTabPath}'");
                return false;
            }

            return true;
        }

        public override bool SendToNautilus()
        {
            QuickLogger.Debug($"CraftingNodePath for {this.Key} '{this.TabID}' set to {this.ParentTabPath}");
            try
            {
                if (this.IsAtRoot)
                {
                    this.ParentFabricator.RootNode.AddTabNode(this.TabID, this.DisplayName, GetCraftingTabSprite());
                }
                else
                {
                    CraftTreePath craftTreePath = GetCraftTreePath();
                    if (craftTreePath.HasError)
                    {
                        QuickLogger.Error($"Encountered error in path for '{this.TabID}' - Entry from {this.Origin} - Error Message: {this.CraftingPath.Error}");
                        return false;
                    }

                    this.ParentFabricator.RootNode.AddTabNode(this.TabID, this.DisplayName, GetCraftingTabSprite(), "English", craftTreePath.StepsToParentTab.LastOrDefault());
                }

                return true;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Exception thrown while handling {this.Key} '{this.TabID}' from {this.Origin}", ex);
                return false;
            }
        }
    }
}
#endif