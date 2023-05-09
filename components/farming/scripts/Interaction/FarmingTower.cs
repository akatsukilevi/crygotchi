namespace Crygotchi;

using System.Collections.Generic;

public partial class FarmingTower : Node3D
{
    [ExportCategory("References")]
    [ExportGroup("Layers")]
    [Export] public Node3D LayersParent;
    [Export] public PackedScene LayerPrefab;
    [ExportGroup("Interaction")]
    [ExportSubgroup("Camera")]
    [Export] public SubViewport InteractionViewport;
    [Export] public FarmingCamera InteractionCamera;
    [ExportSubgroup("GUI")]
    [Export] public PackedScene GUIPrefab;

    private List<FarmingLayer> _layerObjects = new();
    private FarmingInstance _instance;

    public override void _Ready()
    {
        base._Ready();

        this._instance = (FarmingInstance)GetNode<RoomTileObject>("../..").GetDecorationInstance();
        this._instance.OnLayersChange += OnLayersChange;
        this._instance.SetTower(this);
        this.OnLayersChange();
    }

    public void OpenInteraction(Node parent)
    {
        var popup = this.GUIPrefab.Instantiate<FarmingGUI>();
        popup.Ready += () => popup.Setup(this, this.InteractionViewport);

        parent.AddChild(popup);
    }

    public Vector2 ClampInput(Vector2 target)
    {
        if (target.X < 0) target.X = 3; //* Clamp back to highest value
        if (target.X > 3) target.X = 0; //* Clamp forward to smallest value

        if (target.Y < 0) target.Y = this._layerObjects.Count - 1;
        if (target.Y >= this._layerObjects.Count) target.Y = 0;

        return target;
    }

    public void SetCameraTarget(Node3D target, bool skipLerp = false)
    {
        if (skipLerp)
        {
            this.InteractionCamera.SetTarget(target);
            return;
        }

        this.InteractionCamera.LerpTarget(target);
    }

    public Node3D GetPosition(Vector2 position)
    {
        if (this._layerObjects.Count == 0) return this;

        var layer = this._layerObjects.ElementAt((int)position.Y);
        return layer.GetPotPosition((int)position.X);
    }

    private void OnLayersChange()
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
