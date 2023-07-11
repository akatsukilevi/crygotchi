namespace AfterlifeAdventures;

public interface IDatabaseItem
{
    [ExportCategory("Metadata")]
    [Export] public string Name { get; set; }
    [Export] public string Description { get; set; }
    [Export] public Texture2D Icon { get; set; }

    [ExportCategory("Shop Settings")]
    [Export] public bool Purchasable { get; set; }
    [Export] public int Cost { get; set; }
}
