namespace CustomCraft3.Interfaces
{
    internal interface ICustomFood : IAliasRecipe
    {
        FoodModel FoodType { get; }

        short FoodValue { get; }
        short WaterValue { get; }
        short OxygenValue { get; }
        short HealthValue { get; }
#if BELOWZERO
        short HeatValue { get; }
#endif
        float DecayRateMod { get; }
        bool UseDrinkSound { get; }
    }
}