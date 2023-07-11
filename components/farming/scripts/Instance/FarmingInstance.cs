namespace AfterlifeAdventures;

using System.Collections.Generic;

public partial class FarmingInstance : RoomTileDecorationInstance
{
    public event Action OnLayersChange;

    private List<FarmingLayerInstance> _layers = new();
    private List<SeedEntry> _seeds = new();
    private FarmingTower _tower;

    public override Godot.Collections.Dictionary<string, Variant> Serialize()
    {
        Godot.Collections.Array<Variant> layers = new();
        Godot.Collections.Array<Variant> seeds = new();

        foreach (var layer in this._layers) layers.Add(layer.Serialize());
        foreach (var seed in this._seeds) seeds.Add(seed.Serialize());

        return new()
        {
            { "ID", this.ID },
            { "Layers", layers },
            { "Seeds", seeds },
        };
    }

    public override void Deserialize(Godot.Collections.Dictionary<string, Variant> data, TilesDatabase tDB, ItemsDatabase iDB)
    {
        var layers = (Godot.Collections.Array<Variant>)data["Layers"];
        var seeds = (Godot.Collections.Array<Variant>)data["Seeds"];

        this._layers.Clear();
        this._seeds.Clear();

        //* For each layer, deserialize it back into a proper layer
        foreach (var layer in layers)
        {
            var deserialized = FarmingLayerInstance.Deserialize((Godot.Collections.Dictionary<string, Variant>)layer);
            this._layers.Add(deserialized);
        }

        //* For each seed, deserialize it back into a proper seed
        foreach (var seed in seeds)
        {
            var deserialized = SeedEntry.Deserialize((Godot.Collections.Dictionary<string, Variant>)seed);
            deserialized.Seed = (SeedItem)iDB.GetItemById(deserialized.Id);

            this._seeds.Add(deserialized);
        }
    }

    public FarmingInstance()
    {
        //* By start, it will have a single layer
        this._layers = new() { new() { } };
    }

    public void SetTower(FarmingTower tower)
    {
        this._tower = tower;
    }

    public void CreateLayer()
    {
        if (this._layers.Count >= 5) return;

        this._layers.Add(new() { });
        this.OnLayersChange?.Invoke();
    }

    public void RemoveLayer(int index)
    {
        this._layers.RemoveAt(index);
        this.OnLayersChange?.Invoke();
    }

    public FarmingLayerInstance[] GetLayers()
    {
        return this._layers.ToArray();
    }

    public FarmingTower GetTower()
    {
        return this._tower;
    }

    public SeedEntry[] GetSeeds()
    {
        return this._seeds.ToArray();
    }

    public void AddSeed(SeedItem seed)
    {
        int existingIndex = this._seeds.FindIndex(x => x.Id == seed.GetId());
        if (existingIndex != -1)
        {
            //* Already exists, increase the amount
            GD.Print($"Found existing seed {seed.GetId()}, increasing amount to {this._seeds[existingIndex].Amount + 1}");
            this._seeds[existingIndex].Amount += 1;
            return;
        }

        //* Does not exist, add one
        GD.Print($"Couldn't find existing seed {seed.GetId()}, adding new");
        this._seeds.Add(new SeedEntry()
        {
            Id = seed.GetId(),
            Seed = seed,
            Amount = 1
        });
    }

    public SeedItem TakeSeed(string id)
    {
        //* Get the index of it
        int existingIndex = this._seeds.FindIndex(x => x.Id == id);
        if (existingIndex == -1)
        {
            GD.PushError("Could not find seed " + id);
            return null;
        }

        //* Grab the item
        var item = this._seeds[existingIndex].Seed;

        //* Reduce it's amount and remove if it's empty
        this._seeds[existingIndex].Amount -= 1;
        if (this._seeds[existingIndex].Amount <= 0) this._seeds.RemoveAt(existingIndex);

        //* Return it from the grabbed instance
        return item;
    }
}

public class SeedEntry
{
    public string Id { get; set; }
    public SeedItem Seed { get; set; }
    public int Amount { get; set; }

    public Godot.Collections.Dictionary<string, Variant> Serialize()
    {
        return new()
        {
            { "Id", this.Id },
            { "Amount", this.Amount },
        };
    }

    public static SeedEntry Deserialize(Godot.Collections.Dictionary<string, Variant> data)
    {
        return new()
        {
            Id = (string)data["Id"],
            Amount = (int)data["Amount"],
        };
    }
}
