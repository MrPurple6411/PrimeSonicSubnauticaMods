namespace CustomBatteries.API;

using System.Reflection;
using System.Runtime.CompilerServices;

/// <summary>
/// A class that holds all the necessary elements of a custom battery to be patched.
/// </summary>
public class CbBattery : CbItem
{
    /// <summary>
    /// Patches the data of this instance into a new custom Battery.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Patch()
    {
        string name = Assembly.GetCallingAssembly().GetName().Name;
        Patch(ItemTypes.Battery, name);
    }
}
