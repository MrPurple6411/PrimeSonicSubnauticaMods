namespace CustomCraft3.Serialization
{
    using System;
    using System.Collections.Generic;
    using Common;
#if !UNITY_EDITOR
    using Nautilus.Handlers;
#endif

    internal class CraftTreePath
    {
        public const char Separator = '/';

        public CraftTree.Type Scheme { get; private set; }
        public string RawPath { get; }
        public bool IsAtRoot { get; }
        public List<string> RawSteps { get; }
        public string FinalNodeID { get; }
        public string[] StepsToParentTab { get; private set; }
        public string[] StepsToNode { get; private set; }
        public bool HasError { get; private set; }
        public string Error { get; private set; }

        public CraftTreePath(string rawPath, string finalNode)
        {
            this.RawPath = rawPath;
            this.RawSteps = !string.IsNullOrWhiteSpace(rawPath) ? new List<string>(rawPath.Trim(Separator).Split(Separator)) : new List<string>();
            this.IsAtRoot = this.RawSteps.Count <= 1;

            if (string.IsNullOrEmpty(this.RawPath) || this.RawSteps.Count == 0)
            {
                this.HasError = true;
                this.Error = "Empty craft tree path";
                return;
            }

            this.FinalNodeID = finalNode;

            if (string.IsNullOrEmpty(this.FinalNodeID))
            {
                this.HasError = true;
                this.Error = "Missing TabID or ItemID";
                return;
            }

            this.Scheme = GetCraftTreeType(this.RawSteps[0]);

            if (this.Scheme == CraftTree.Type.None)
            {
                this.HasError = true;
                this.Error = $"Unable to identify fabricator {this.RawSteps[0]}";
                return;
            }

            this.StepsToParentTab = StepsToParentAdding();
            this.StepsToNode = StepsToNodeRemoving();

            this.HasError = false;
        }

        public void ReCheck()
        {
            if (string.IsNullOrEmpty(this.RawPath) || this.RawSteps.Count == 0)
            {
                this.HasError = true;
                this.Error = "Empty craft tree path";
                return;
            }

            if (string.IsNullOrEmpty(this.FinalNodeID))
            {
                this.HasError = true;
                this.Error = "Missing TabID or ItemID";
                return;
            }

            this.Scheme = GetCraftTreeType(this.RawSteps[0]);

            if (this.Scheme == CraftTree.Type.None)
            {
                this.HasError = true;
                this.Error = $"Unable to identify fabricator {this.RawSteps[0]}";
                return;
            }

            this.StepsToParentTab = StepsToParentAdding();
            this.StepsToNode = StepsToNodeRemoving();
            this.HasError = false;
        }

        private string[] StepsToParentAdding()
        {
            if (this.IsAtRoot)
                return null;

            var copy = new List<string>(this.RawSteps);
            copy.RemoveAt(0);
            return copy.ToArray();
        }

        private string[] StepsToNodeRemoving()
        {
            if (this.IsAtRoot)
                return new[] { this.FinalNodeID };

            var copy = new List<string>(this.RawSteps)
        {
            this.FinalNodeID
        };
            copy.RemoveAt(0);
            return copy.ToArray();
        }

        internal static CraftTree.Type GetCraftTreeType(string schemeString)
        {
            if (Enum.TryParse(schemeString, true, out CraftTree.Type type))
            {
                return type;
            }
#if !UNITY_EDITOR
            if (EnumHandler.TryGetValue(schemeString, out type))
            {
                return type;
            }
#endif

            return CraftTree.Type.None;
        }
    }
}