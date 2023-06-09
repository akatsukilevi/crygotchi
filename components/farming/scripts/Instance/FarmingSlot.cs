namespace Crygotchi;

public partial class FarmingSlot : Resource
{
    private GrowthStage _growth = GrowthStage.Stage0;
    private SlotState _state = SlotState.Plantable;
    private float _growthTime = 0f;
    private float _dryTime = 0f;
    private bool _isDry = true;
    private SeedItem _seed;

    //* Setup states
    private bool _hasSetupDetails = false;
    private bool _hasSetupSlot = false;

    //* UI references
    private TreeItem _detailStatus;
    private TreeItem _detailGrowthTime;
    private TreeItem _detailGrowthStage;
    private TreeItem _detailDryess;

    //* World references
    private Node3D DirtDry;
    private Node3D DirtWet;
    private Node3D Mountpoint;
    private Node3D CamTarget;
    private Node3D Plant_Stage0;
    private Node3D Plant_Stage1;
    private Node3D Plant_Stage2;
    private Node3D Plant_Stage3;

    public Godot.Collections.Dictionary<string, Variant> Serialize()
    {
        return new()
        {
            { "Stage", (int)this._growth },
            { "State", (int)this._state },
            { "GrowthTime", this._growthTime },
            { "DryTime", this._dryTime },
            { "IsDry", this._isDry },
            { "SeedID", this._seed },
        };
    }

    public void Setup(string Name, Node3D PotBase, Node3D LayerBase, PackedScene PlantPrefab)
    {
        var PlantA = PlantPrefab.Instantiate();
        this.DirtDry = PotBase.GetNode<Node3D>($"./{Name}_DirtDry");
        this.DirtWet = PotBase.GetNode<Node3D>($"./{Name}_DirtWet");
        this.Mountpoint = PotBase.GetNode<Node3D>($"./{Name}_Mountpoint");
        this.CamTarget = LayerBase.GetNode<Node3D>($"./{Name}_CamTarget");

        this.Mountpoint.AddChild(PlantA);
        this.Plant_Stage0 = PlantA.GetNode<Node3D>("./Plant_Stage0");
        this.Plant_Stage1 = PlantA.GetNode<Node3D>("./Plant_Stage1");
        this.Plant_Stage2 = PlantA.GetNode<Node3D>("./Plant_Stage2");
        this.Plant_Stage3 = PlantA.GetNode<Node3D>("./Plant_Stage3");

        this._hasSetupSlot = true;
    }

    public void SetupDetails(Tree detailsTree, TreeItem root)
    {
        this._hasSetupDetails = false;

        this._detailStatus = root;
        this._detailGrowthStage = detailsTree.CreateItem(root);
        this._detailGrowthTime = detailsTree.CreateItem(root);
        this._detailDryess = detailsTree.CreateItem(root);

        this._detailGrowthStage.SetText(0, "Growth Stage");
        this._detailGrowthTime.SetText(0, "Growth Time");
        this._detailDryess.SetText(0, "Dryness");

        this._hasSetupDetails = true;
    }

    public void Update()
    {
        if (!this._hasSetupSlot) return;

        if (this._state == SlotState.Growing) this.UpdateGrowthState();
        if (this._hasSetupDetails) this.UpdateDetails();
        this.UpdateVisual();
    }

    public Node3D GetCameraTarget()
    {
        return this.CamTarget;
    }

    public SlotState GetState()
    {
        return this._state;
    }

    public void TryAddWater()
    {
        if (this._state != SlotState.Growing) return;
        if (!this._isDry) return;

        this._dryTime = this._seed.DrySeconds;
        this._isDry = false;
    }

    public void PlantSeed(SeedItem seed)
    {
        this._growthTime = seed.GrowthSeconds;
        this._growth = GrowthStage.Stage0;
        this._seed = seed;

        this._state = SlotState.Growing;
    }

    public FoodItem Harvest()
    {
        var harvested = this._seed.Food;

        this._growth = GrowthStage.Stage0;
        this._state = SlotState.Plantable;
        this._growthTime = 0f;
        this._seed = null;

        return harvested;
    }

    public void UIClose()
    {
        this._hasSetupDetails = false;

        this._detailStatus = null;
        this._detailGrowthTime = null;
        this._detailGrowthStage = null;
        this._detailDryess = null;
    }

    private void UpdateVisual()
    {
        //* Update the ground
        this.DirtDry.Visible = this._isDry;
        this.DirtWet.Visible = !this._isDry;

        //* Update the plant stages
        switch (this._state)
        {
            case SlotState.Plantable:
                this.Plant_Stage0.Visible = false;
                this.Plant_Stage1.Visible = false;
                this.Plant_Stage2.Visible = false;
                this.Plant_Stage3.Visible = false;
                break;
            case SlotState.Growing:
                this.Plant_Stage0.Visible = this._growth == GrowthStage.Stage0;
                this.Plant_Stage1.Visible = this._growth == GrowthStage.Stage1;
                this.Plant_Stage2.Visible = this._growth == GrowthStage.Stage2;
                this.Plant_Stage3.Visible = this._growth == GrowthStage.Stage3;
                break;
            case SlotState.Harvestable:
                this.Plant_Stage0.Visible = false;
                this.Plant_Stage1.Visible = false;
                this.Plant_Stage2.Visible = false;
                this.Plant_Stage3.Visible = true;
                break;
        }
    }

    private void UpdateDetails()
    {
        this._detailStatus.SetText(1, this._state switch
        {
            SlotState.Growing => "Growing",
            SlotState.Harvestable => "Harvestable",
            _ => "Plantable",
        });

        this._detailDryess.SetText(1, this._isDry ? "Dry" : $"Wet ({(int)this._dryTime}s)");

        if (this._state == SlotState.Growing)
        {
            this._detailGrowthStage.SetText(1, this._growth switch
            {
                GrowthStage.Stage0 => "Stage 1",
                GrowthStage.Stage1 => "Stage 2",
                GrowthStage.Stage2 => "Stage 3",
                _ => "Stage 4",
            });

            this._detailGrowthTime.SetText(1, $"{(int)this._growthTime}s");

            this._detailGrowthStage.Visible = true;
            this._detailGrowthTime.Visible = true;
            return;
        }

        this._detailGrowthStage.SetText(1, "N/A");
        this._detailGrowthTime.SetText(1, "N/A");

        this._detailGrowthStage.Visible = false;
        this._detailGrowthTime.Visible = false;
    }

    private void UpdateGrowthState()
    {
        //* Update the timers
        if (!this._isDry) this._dryTime -= 0.075f;
        if (!this._isDry) this._growthTime -= 0.075f;

        //* Update the state
        if (!this._isDry && this._dryTime <= 0f) this._isDry = true; //* Mark as dry

        //* If didn't finsihed growing yet, stop here
        if (this._growthTime > 0f) return;

        //* If already on the final growth stage, means it should be harvestable now
        if (this._growth == GrowthStage.Stage3) this._state = SlotState.Harvestable;

        this._growth = this._growth switch
        {
            GrowthStage.Stage0 => GrowthStage.Stage1,
            GrowthStage.Stage1 => GrowthStage.Stage2,
            GrowthStage.Stage2 => GrowthStage.Stage3,
            _ => GrowthStage.Stage0,
        };
    }
}

public enum SlotState
{
    Plantable,
    Growing,
    Harvestable
}

public enum GrowthStage
{
    Stage0,
    Stage1,
    Stage2,
    Stage3
}
