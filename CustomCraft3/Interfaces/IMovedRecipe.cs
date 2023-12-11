namespace CustomCraft3.Interfaces
{
    public interface IMovedRecipe : ITechTyped
    {
        string OldPath { get; }
        string NewPath { get; }
        bool Hidden { get; }
        bool Copied { get; }
    }
}