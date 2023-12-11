namespace CustomCraft3.Interfaces.InternalUse
{
    using CustomCraft3.Serialization;

    internal interface IParsingPackage
    {
        string ListKey { get; }
        string TypeName { get; }
        string[] TutorialText { get; }
#if !UNITY_EDITOR
        int ParseEntries(string serializedData, OriginFile file);
        void PrePassValidation();
        void SecondPassValidation();
        void SendToNautilus();
#endif
    }
}