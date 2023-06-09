namespace Crygotchi;

using System.Collections.Generic;

public partial class ItemsDatabase : Node
{
    private readonly Dictionary<string, Item> _items;

    public ItemsDatabase() : base()
    {
        var loaded = FileSystemUtils.LoadAll<Item>("res://resources/items");
        this._items = new();

        foreach (var item in loaded)
        {
            var id = ResourceUid.IdToText(ResourceLoader.GetResourceUid(item.ResourcePath));
            var path = item.ResourcePath;

            if (id.Contains("<invalid>"))
            {
                GD.PrintErr($"Invalid ID for tile {path}");
                continue;
            }

            if (this._items.ContainsKey(id))
            {
                GD.PrintErr($"Cannot add duplicated item \"{id}\" ({path})");
                continue;
            }

            GD.Print($"Loading in item \"{id}\" ({path})");
            this._items.Add(id, item.Setup(id));
        }
    }

    public Item GetItemById(string id)
    {
        if (this._items.TryGetValue(id, out Item item)) return item;

        GD.PushWarning($"Cannot find item \"{id}\"");
        return null;
    }
}
