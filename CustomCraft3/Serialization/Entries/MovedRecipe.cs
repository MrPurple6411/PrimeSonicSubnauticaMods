#if !UNITY_EDITOR
namespace CustomCraft3.Serialization.Entries
{
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using CustomCraft3.Interfaces;
    using CustomCraft3.Interfaces.InternalUse;
    using CustomCraft3.Serialization;
    using CustomCraft3.Serialization.Components;
    using Nautilus.Handlers;

    internal partial class MovedRecipe : EmTechTyped, IMovedRecipe, ICustomCraft
    {
        public override bool PassesPreValidation(OriginFile originFile)
        {
            return IsValidState() & base.PassesPreValidation(originFile);
        }

        private bool IsValidState()
        {
            if (!this.Copied && string.IsNullOrEmpty(this.OldPath))
            {
                if (!PassedPreValidation)
                    QuickLogger.Warning($"{OldPathKey} missing while {CopyKey} was not set to 'YES' in {this.Key} for '{this.ItemID}' from {this.Origin}");
                return false;
            }

            if (this.Copied && this.Hidden)
            {
                if (!PassedPreValidation)
                    QuickLogger.Warning($"Invalid request in {this.Key} for '{this.ItemID}' from {this.Origin}. {CopyKey} and {HiddenKey} cannot both be set to 'YES'");
                return false;
            }

            if (string.IsNullOrEmpty(this.NewPath) && (this.Copied || !this.Hidden))
            {
                if (!PassedPreValidation)
                    QuickLogger.Warning($"{NewPathKey} value missing in {this.Key} for '{this.ItemID}' from {this.Origin}");
                return false;
            }

            return true;
        }

        public bool SendToNautilus()
        {
            if (this.Hidden || !this.Copied || AlternatePaths.Any(x => x.Value.Hidden || !this.Copied))
            {
                var oldPath = new CraftTreePath(this.OldPath, this.ItemID);
                if (!oldPath.HasError)
                {
                    CraftTreeHandler.RemoveNode(oldPath.Scheme, oldPath.StepsToNode);
                    QuickLogger.Debug($"Removed crafting node at '{this.ItemID}' - Entry from {this.Origin}");
                }
            }

            HandleCraftTreeAddition();

            return true;
        }

        protected virtual void HandleCraftTreeAddition()
        {
            var paths = new Dictionary<CraftTreePath, OriginFile>
            {

            };

            if (!this.Hidden)
            {
                paths.Add(new CraftTreePath(this.NewPath, this.ItemID), this.Origin);
            }

            foreach (var pair in this.AlternatePaths)
            {
                if (pair.Value.Hidden)
                    continue;
                paths.Add(new CraftTreePath(pair.Value.NewPath, this.ItemID), pair.Key);
            }

            foreach (var pair in paths)
            {
                if (!pair.Key.HasError)
                {
                    AddCraftNode(pair.Key, this.TechType, pair.Value);
                }
                else
                {
                    FailedPaths.Add(pair.Key, pair.Value);
                    MovedRecipesWithErrors.Add(this);
                }
            }
        }

        internal void RetryFailedPaths()
        {
            foreach (var path in this.FailedPaths)
            {
                path.Key.ReCheck();
                if (path.Key.HasError)
                {
                    // Log that this path failed and that it will be discarded
                    QuickLogger.Warning($"Unable to move recipe to path '{path.Key.RawPath}' for '{this.ItemID}' from {path.Value} because {path.Key.Error}");
                    continue;
                }
                AddCraftNode(path.Key, this.TechType, path.Value);
            }
        }
    }
}
#endif