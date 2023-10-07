namespace CustomCraft3.Interfaces;

internal interface ICustomFood : IAliasRecipe
{
    FoodModel FoodType { get; }

    short FoodValue { get; }
    short WaterValue { get; }
    float DecayRateMod { get; }
    bool UseDrinkSound { get; }
}
