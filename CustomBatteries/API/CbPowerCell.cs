namespace CustomBatteries.API;

using System.Reflection;
using System.Runtime.CompilerServices;

/// <summary>
/// A class that holds all the necessary elements of a custom power cell to be patched.
/// </summary>
public class CbPowerCell : CbItem
{
    /// <summary>
    /// Patches the data of this instance into a new custom Power Cell.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Patch()
    {
        string name = Assembly.GetCallingAssembly().GetName().Name;
        Patch(ItemTypes.PowerCell, name);
    }
}
