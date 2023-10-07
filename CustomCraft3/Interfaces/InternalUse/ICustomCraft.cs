namespace CustomCraft3.Interfaces.InternalUse;

using CustomCraft3.Serialization;

internal interface ICustomCraft
{
    string ID { get; }
    bool PassesPreValidation(OriginFile originFile);
    bool PassedSecondValidation { get; }
    bool SendToSMLHelper();

    OriginFile Origin { get; set; }
    string[] TutorialText { get; }
}