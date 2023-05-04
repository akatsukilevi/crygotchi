namespace Crygotchi;

public partial class FarmingLayer : Node3D
{
    [ExportCategory("Visuals")]
    [Export] public Node3D FirstSlot;
    [Export] public Node3D SecondSlot;
    [Export] public Node3D ThirdSlot;
    [Export] public Node3D FourthSlot;

    private FarmingLayerInstance _instance;

    public void Setup(FarmingLayerInstance layerInstance, int index = 0)
    {
        this._instance = layerInstance;
        this.Position = new Vector3(0f, 0.3f * index, 0f);
        if (index % 2 != 0) this.RotateY(45f);
    }
}
