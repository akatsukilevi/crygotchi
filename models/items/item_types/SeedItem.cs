namespace AfterlifeAdventures;

public partial class SeedItem : Item
{
    [ExportCategory("Farming")]
    [ExportGroup("Planting")]
    [Export] public float GrowthSeconds = 60f;
    [Export] public float DrySeconds = 30f;
    [ExportGroup("Harvesting")]
    [Export] public FoodItem Food;
}
