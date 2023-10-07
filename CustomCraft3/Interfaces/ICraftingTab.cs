namespace CustomCraft3.Interfaces;

public interface ICraftingTab
{
    string TabID { get; }
    string DisplayName { get; }
    TechType SpriteItemID { get; }
    string ParentTabPath { get; }
}
