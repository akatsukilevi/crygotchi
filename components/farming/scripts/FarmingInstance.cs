namespace Crygotchi;

using System.Collections.Generic;

public partial class FarmingInstance : RoomTileDecorationInstance
{
    public List<FarmingLayerInstance> _layers = new();
    public event EventHandler OnLayersChange;

    public void CreateLayer()
    {
        if (this._layers.Count >= 5) return;

        this._layers.Add(new() { });
        this.OnLayersChange?.Invoke(this, null);
    }

    public void RemoveLayer(int index)
    {
        this._layers.RemoveAt(index);
        this.OnLayersChange?.Invoke(this, null);
    }

    public FarmingLayerInstance[] GetLayers()
    {
        return this._layers.ToArray();
    }
}
