namespace Crygotchi;

public partial class FarmingLayer : Node3D
{
    [ExportCategory("Visuals")]
    [Export] public Node3D LayerMesh;
    [Export] public PackedScene PlantPrefab;

    private FarmingLayerInstance _instance;

    //* Pot A
    private Node3D PotA;
    private Node3D PotA_DirtDry;
    private Node3D PotA_DirtWet;
    private Node3D PotA_Mountpoint;
    private Node3D PotA_CamTarget;
    private Node3D PotA_Plant_Stage0;
    private Node3D PotA_Plant_Stage1;
    private Node3D PotA_Plant_Stage2;
    private Node3D PotA_Plant_Stage3;

    //* Pot B
    private Node3D PotB;
    private Node3D PotB_DirtDry;
    private Node3D PotB_DirtWet;
    private Node3D PotB_Mountpoint;
    private Node3D PotB_CamTarget;
    private Node3D PotB_Plant_Stage0;
    private Node3D PotB_Plant_Stage1;
    private Node3D PotB_Plant_Stage2;
    private Node3D PotB_Plant_Stage3;

    //* Pot C
    private Node3D PotC;
    private Node3D PotC_DirtDry;
    private Node3D PotC_DirtWet;
    private Node3D PotC_Mountpoint;
    private Node3D PotC_CamTarget;
    private Node3D PotC_Plant_Stage0;
    private Node3D PotC_Plant_Stage1;
    private Node3D PotC_Plant_Stage2;
    private Node3D PotC_Plant_Stage3;

    //* Pot D
    private Node3D PotD;
    private Node3D PotD_DirtDry;
    private Node3D PotD_DirtWet;
    private Node3D PotD_Mountpoint;
    private Node3D PotD_CamTarget;
    private Node3D PotD_Plant_Stage0;
    private Node3D PotD_Plant_Stage1;
    private Node3D PotD_Plant_Stage2;
    private Node3D PotD_Plant_Stage3;

    public override void _Ready()
    {
        base._Ready();

        //* Pot A
        var PlantA = this.PlantPrefab.Instantiate();
        this.PotA = this.LayerMesh.GetNode<Node3D>("./PotA");
        this.PotA_DirtDry = this.PotA.GetNode<Node3D>("./PotA_DirtDry");
        this.PotA_DirtWet = this.PotA.GetNode<Node3D>("./PotA_DirtWet");
        this.PotA_Mountpoint = this.PotA.GetNode<Node3D>("./PotA_Mountpoint");
        this.PotA_CamTarget = this.GetNode<Node3D>("./PotA_CamTarget");

        this.PotA_Mountpoint.AddChild(PlantA);
        this.PotA_Plant_Stage0 = PlantA.GetNode<Node3D>("./Plant_Stage0");
        this.PotA_Plant_Stage1 = PlantA.GetNode<Node3D>("./Plant_Stage1");
        this.PotA_Plant_Stage2 = PlantA.GetNode<Node3D>("./Plant_Stage2");
        this.PotA_Plant_Stage3 = PlantA.GetNode<Node3D>("./Plant_Stage3");

        //* Pot B
        var PlantB = this.PlantPrefab.Instantiate();
        this.PotB = this.LayerMesh.GetNode<Node3D>("./PotB");
        this.PotB_DirtDry = this.PotB.GetNode<Node3D>("./PotB_DirtDry");
        this.PotB_DirtWet = this.PotB.GetNode<Node3D>("./PotB_DirtWet");
        this.PotB_Mountpoint = this.PotB.GetNode<Node3D>("./PotB_Mountpoint");
        this.PotB_CamTarget = this.GetNode<Node3D>("./PotB_CamTarget");

        this.PotB_Mountpoint.AddChild(PlantB);
        this.PotB_Plant_Stage0 = PlantB.GetNode<Node3D>("./Plant_Stage0");
        this.PotB_Plant_Stage1 = PlantB.GetNode<Node3D>("./Plant_Stage1");
        this.PotB_Plant_Stage2 = PlantB.GetNode<Node3D>("./Plant_Stage2");
        this.PotB_Plant_Stage3 = PlantB.GetNode<Node3D>("./Plant_Stage3");

        //* Pot C
        var PlantC = this.PlantPrefab.Instantiate();
        this.PotC = this.LayerMesh.GetNode<Node3D>("./PotC");
        this.PotC_DirtDry = this.PotC.GetNode<Node3D>("./PotC_DirtDry");
        this.PotC_DirtWet = this.PotC.GetNode<Node3D>("./PotC_DirtWet");
        this.PotC_Mountpoint = this.PotC.GetNode<Node3D>("./PotC_Mountpoint");
        this.PotC_CamTarget = this.GetNode<Node3D>("./PotC_CamTarget");

        this.PotC_Mountpoint.AddChild(PlantC);
        this.PotC_Plant_Stage0 = PlantC.GetNode<Node3D>("./Plant_Stage0");
        this.PotC_Plant_Stage1 = PlantC.GetNode<Node3D>("./Plant_Stage1");
        this.PotC_Plant_Stage2 = PlantC.GetNode<Node3D>("./Plant_Stage2");
        this.PotC_Plant_Stage3 = PlantC.GetNode<Node3D>("./Plant_Stage3");

        //* Pot D
        var PlantD = this.PlantPrefab.Instantiate();
        this.PotD = this.LayerMesh.GetNode<Node3D>("./PotD");
        this.PotD_DirtDry = this.PotD.GetNode<Node3D>("./PotD_DirtDry");
        this.PotD_DirtWet = this.PotD.GetNode<Node3D>("./PotD_DirtWet");
        this.PotD_Mountpoint = this.PotD.GetNode<Node3D>("./PotD_Mountpoint");
        this.PotD_CamTarget = this.GetNode<Node3D>("./PotD_CamTarget");

        this.PotD_Mountpoint.AddChild(PlantD);
        this.PotD_Plant_Stage0 = PlantD.GetNode<Node3D>("./Plant_Stage0");
        this.PotD_Plant_Stage1 = PlantD.GetNode<Node3D>("./Plant_Stage1");
        this.PotD_Plant_Stage2 = PlantD.GetNode<Node3D>("./Plant_Stage2");
        this.PotD_Plant_Stage3 = PlantD.GetNode<Node3D>("./Plant_Stage3");
    }

    public void Setup(FarmingLayerInstance layerInstance, int index = 0)
    {
        this._instance = layerInstance;
        this.Position = new Vector3(0f, 0.3f * index, 0f);
        if (index % 2 != 0) this.RotateY(Mathf.DegToRad(45f));
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        //* Update individual pots
        this._instance.FirstSlot.Update(this.PotA_DirtDry, this.PotA_DirtWet);
        this._instance.SecondSlot.Update(this.PotB_DirtDry, this.PotB_DirtWet);
        this._instance.ThirdSlot.Update(this.PotC_DirtDry, this.PotC_DirtWet);
        this._instance.FourthSlot.Update(this.PotD_DirtDry, this.PotD_DirtWet);
    }

    public Node3D GetPotPosition(int position)
    {
        if (position == 0) return this.PotA_CamTarget;
        if (position == 1) return this.PotB_CamTarget;
        if (position == 2) return this.PotC_CamTarget;
        if (position == 3) return this.PotD_CamTarget;

        GD.PushWarning("Tried to fetch invalid pot " + position);
        return null;
    }
}
