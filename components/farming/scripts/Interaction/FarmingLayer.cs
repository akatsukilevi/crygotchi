namespace AfterlifeAdventures;

public partial class FarmingLayer : Node3D
{
    [ExportCategory("Visuals")]
    [Export] public Node3D LayerMesh;
    [Export] public PackedScene PlantPrefab;

    private FarmingLayerInstance _instance;
    private bool _isSetup = false;

    //* Pots
    private Node3D PotA;
    private Node3D PotB;
    private Node3D PotC;
    private Node3D PotD;

    public override void _Ready()
    {
        base._Ready();

        //* Get pot meshes
        this.PotA = this.LayerMesh.GetNode<Node3D>("./PotA");
        this.PotB = this.LayerMesh.GetNode<Node3D>("./PotB");
        this.PotC = this.LayerMesh.GetNode<Node3D>("./PotC");
        this.PotD = this.LayerMesh.GetNode<Node3D>("./PotD");
    }

    public void Setup(FarmingLayerInstance layerInstance, int index = 0)
    {
        this._instance = layerInstance;
        this.Position = new Vector3(0f, 0.3f * index, 0f);
        if (index % 2 != 0) this.RotateY(Mathf.DegToRad(45f));

        //* Setup pot instances
        this._instance.FirstSlot.Setup("PotA", this.PotA, this, this.PlantPrefab);
        this._instance.SecondSlot.Setup("PotB", this.PotB, this, this.PlantPrefab);
        this._instance.ThirdSlot.Setup("PotC", this.PotC, this, this.PlantPrefab);
        this._instance.FourthSlot.Setup("PotD", this.PotD, this, this.PlantPrefab);

        //* Finish setup
        this._isSetup = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!this._isSetup) return;

        //* Update individual pots
        this._instance.FirstSlot.Update();
        this._instance.SecondSlot.Update();
        this._instance.ThirdSlot.Update();
        this._instance.FourthSlot.Update();
    }

    public Node3D GetPotPosition(int position)
    {
        if (position == 0) return this._instance.FirstSlot.GetCameraTarget();
        if (position == 1) return this._instance.SecondSlot.GetCameraTarget();
        if (position == 2) return this._instance.ThirdSlot.GetCameraTarget();
        if (position == 3) return this._instance.FourthSlot.GetCameraTarget();

        throw new System.Exception("Tried to fetch invalid pot " + position);
    }

    public FarmingSlot GetSlot(int slot)
    {
        if (slot == 0) return this._instance.FirstSlot;
        if (slot == 1) return this._instance.SecondSlot;
        if (slot == 2) return this._instance.ThirdSlot;
        if (slot == 3) return this._instance.FourthSlot;

        throw new System.Exception("Tried to fetch invalid pot " + slot);
    }
}
