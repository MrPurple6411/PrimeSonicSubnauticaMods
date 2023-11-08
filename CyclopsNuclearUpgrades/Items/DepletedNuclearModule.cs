namespace CyclopsNuclearUpgrades;

using System.IO;
using System.Reflection;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;

internal static class DepletedNuclearModule
{
    private const string ClassId = "DepletedCyclopsNuclearModule";
    private const string DisplayName = "Depleted Cyclops Nuclear Reactor Module";
    private const string Description = "Nuclear waste.";
    private const TechType depletedReactorRod = TechType.DepletedReactorRod;

    public static string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

    public static PrefabInfo Info { get; private set; }
    public static CustomPrefab CustomPrefab { get; private set; }

    internal static void CreateAndRegister()
    {
        // Creates the PrefabInfo
        Info = PrefabInfo.WithTechType(ClassId, DisplayName, Description);
        var spritePath = Path.Combine(AssetsFolder, $"{ClassId}.png");

        if(File.Exists(spritePath))
            Info.WithIcon(ImageUtils.LoadSpriteFromFile(spritePath));
        else
            Info.WithIcon(SpriteManager.Get(depletedReactorRod));

        CustomPrefab = new CustomPrefab(Info);
        CustomPrefab.SetGameObject(new CloneTemplate(Info, depletedReactorRod) { ModifyPrefab = (go) => go.SetActive(false) });

        CustomPrefab.Register();
    }
}
