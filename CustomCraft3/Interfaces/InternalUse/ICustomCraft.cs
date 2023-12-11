namespace CustomCraft3.Interfaces.InternalUse
{
    using CustomCraft3.Serialization;

    internal interface ICustomCraft
    {
        string ID { get; }
        bool PassedPreValidation { get; }
        bool PassedSecondValidation { get; }
        OriginFile Origin { get; set; }
        string[] TutorialText { get; }

#if !UNITY_EDITOR
        bool PassesPreValidation(OriginFile originFile);
        bool SendToNautilus();
#endif
    }
}