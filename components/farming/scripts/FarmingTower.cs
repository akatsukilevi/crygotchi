namespace Crygotchi;

using System.Collections.Generic;

public partial class FarmingTower : Node3D
{
    [ExportCategory("References")]
    [Export] public Node3D LayersParent;
    [Export] public PackedScene LayerPrefab;

    private List<FarmingLayer> _layerObjects = new();
    private FarmingInstance _instance;

    public override void _Ready()
    {
        base._Ready();

        this._instance = (FarmingInstance)GetNode<RoomTileObject>("../..").GetDecorationInstance();
        this._instance.OnLayersChange += OnLayersChange;
    }

    private void OnLayersChange(object sender, EventArgs e)
    {
        //* Remove existing layers
        foreach (var layer in this._layerObjects) layer.QueueFree();
        this._layerObjects.Clear();

        //* Create new layers
        var newLayers = this._instance.GetLayers();
        int index = 0;
        foreach (var layer in newLayers)
        {
            //* Should instantiate a layer per instance
            var newLayer = LayerPrefab.Instantiate<FarmingLayer>();
            newLayer.Ready += () => newLayer.Setup(layer, index);
            this._layerObjects.Add(newLayer);

            this.LayersParent.AddChild(newLayer);
            index += 1;
        }
    }
}
