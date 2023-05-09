namespace Crygotchi;

using System.Collections.Generic;

public partial class FarmingInstance : RoomTileDecorationInstance
{
    public List<FarmingLayerInstance> _layers = new();
    public event Action OnLayersChange;
    private FarmingTower _tower;

    public FarmingInstance()
    {
        //! DEBUG ONLY
        // TODO: Remove this...
        this._layers = new() {
            new() {},
            new() {},
            new() {},
        };
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

    public void SetTower(FarmingTower tower)
    {
        this._tower = tower;
    }

    public FarmingTower GetTower()
    {
        return this._tower;
    }
}
