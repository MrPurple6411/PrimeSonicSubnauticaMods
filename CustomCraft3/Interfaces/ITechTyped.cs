namespace CustomCraft3.Interfaces
{
    public interface ITechTyped
    {
        string ItemID { get; }
        string Key { get; }
        TechType TechType { get; set; }
    }
}