namespace CustomCraft3.Interfaces
{
    internal interface IAliasRecipe : IAddedRecipe
    {
        string DisplayName { get; }
        string Tooltip { get; }
        string FunctionalID { get; }
        TechType SpriteItemID { get; }
    }
}