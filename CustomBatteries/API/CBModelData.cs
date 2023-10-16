namespace CustomBatteries.API;

using Nautilus.Assets;

public class CBModelData: CustomModelData
{
    /// <summary>
    /// Change this value if you want your item to use the Ion battery or powercell model as its base.
    /// </summary>
    public bool UseIonModelsAsBase { get; set; } = false;
}
