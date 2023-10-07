namespace CustomCraft3.Interfaces;

internal interface IAddedRecipe : IModifiedRecipe
{
    string Path { get; }
    TechGroup PdaGroup { get; }
    TechCategory PdaCategory { get; }
}