namespace CustomCraft3.Interfaces.InternalUse;

using CustomCraft3.Serialization;

internal interface IParsingPackage
{
    int ParseEntries(string serializedData, OriginFile file);
    void PrePassValidation();
    void SendToSMLHelper();
    string ListKey { get; }
    string TypeName { get; }
    string[] TutorialText { get; }
}