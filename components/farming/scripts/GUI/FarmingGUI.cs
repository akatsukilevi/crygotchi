using System.Collections.Generic;

namespace Crygotchi;

public partial class FarmingGUI : Panel
{
    [Export] public TextureRect Viewer;
    [Export] public Label ToolLabel;
    [Export] public ItemList List;
    [Export] public Tree Details;

    private Vector2 _focused = Vector2.Zero;
    private bool _isStorageOpen = false;
    private Tool _tool = Tool.Trowel;
    private bool _hasSetup = false;
    private bool _shouldClose = false;
    private SeedEntry[] _seeds;

    private List<LayerTree> _tree = new();
    private FarmingInstance _instance;
    private CursorState _cursorState;
    private FarmingTower _tower;
    private OSCController _osc;

    public override void _Ready()
    {
        base._Ready();

        this._cursorState = this.GetNode<CursorState>("/root/CursorState");
        this._osc = this.GetNode<OSCController>("/root/OSCController");
        this._cursorState.SetBusy(true);
    }

    public void Setup(FarmingTower tower, SubViewport viewport)
    {
        if (this._hasSetup) return;

        //* Set instance references
        this._instance = tower.GetInstance();
        this._tower = tower;

        //* Set the camera initial target
        this._tower.SetCameraTarget(this._tower.GetPosition(this._focused), true);

        //* Add event hooks and assign view texture
        this.Viewer.Texture = viewport.GetTexture();
        this.List.ItemActivated += OnSeedActivated;

        //* Reset state to ensure it is right
        this._focused = Vector2.Zero;
        this._isStorageOpen = false;
        this._tool = Tool.Trowel;

        //* Initial input & Finish setup
        this.SetupDetails();
        this.OnInputChange(Vector2.Zero);

        this._hasSetup = true;
        this.ProcessInput();
    }

    private void ProcessInput()
    {
        if (!this._hasSetup) return;

        var keys = new List<OSC>()
        {
            new() {
                Key = OSCKey.Cancel,
                Name = "Close",
                OnActivate = () => {
                    this._shouldClose = true;
                }
            },
            new() {
                Key = OSCKey.Primary,
                Name = this._tool == Tool.Trowel ? "Plant" : "Water",
                OnActivate = () => {
                    switch (this._tool)
                    {
                        case Tool.Trowel:
                            this.InteractTrowel();
                            break;
                        case Tool.WaterCan:
                            this.InteractWaterCan();
                            break;
                    }
                }
            }
        };

        if (this._tool == Tool.Trowel && this._isStorageOpen)
        {
            keys.Add(new()
            {
                Key = OSCKey.Secondary,
                Name = "Cancel Planting",
                OnActivate = this.CancelTrowel
            });
        }

        if (!this._isStorageOpen)
        {
            keys.AddRange(new OSC[] {
                new DirectionalOSC() {
                    Key = OSCKey.Axis,
                    Name = "Move",
                    OnActivate = this.OnInputChange
                },
                new OSC() {
                    Key = OSCKey.ShiftLeft,
                    Name = "Previous Tool",
                    OnActivate = this.SwitchTool
                },
                new OSC() {
                    Key = OSCKey.ShiftRight,
                    Name = "Next Tool",
                    OnActivate = this.SwitchTool
                }
            });
        }

        this._osc.RegisterOSC(keys.ToArray());
    }

    public override void _Process(double delta)
    {
        base._PhysicsProcess(delta);

        if (this._shouldClose)
        {
            this.Close();
            return;
        }

        //* Update the camera target
        var targetPosition = this._tower.GetPosition(this._focused);
        this._tower.SetCameraTarget(targetPosition, false);

        this.List.Visible = this._isStorageOpen;
        this.Details.Visible = !this._isStorageOpen;

        if (this._isStorageOpen)
        {
            this.List.GrabFocus();
            this.List.GrabClickFocus();
        }

        this.ToolLabel.Text = this._tool switch
        {
            Tool.Trowel => "Trowel",
            _ => "Water Can",
        };
    }

    private void SwitchTool()
    {
        this._tool = this._tool switch
        {
            Tool.Trowel => Tool.WaterCan,
            _ => Tool.Trowel
        };

        this.ProcessInput();
    }

    private void InteractTrowel()
    {
        var slot = this._tower.GetSlot(this._focused);
        var state = slot.GetState();

        switch (state)
        {
            case SlotState.Plantable:
                this.OpenSeedStorage();
                break;
            case SlotState.Harvestable:
                this.HarvestSlot(slot);
                break;
        }
    }

    private void HarvestSlot(FarmingSlot slot)
    {
        var harvested = slot.Harvest();
        this._cursorState.HoldItem(harvested);
        this._shouldClose = true;
    }

    private void InteractWaterCan()
    {
        if (this._isStorageOpen) return;

        var slot = this._tower.GetSlot(this._focused);
        slot.TryAddWater();
    }

    private void CancelTrowel()
    {
        this._isStorageOpen = false;
    }

    private void SetupDetails()
    {
        //* Setup configuration
        this.Details.HideFolding = true;
        this.Details.HideRoot = true;
        this.Details.Clear();
        this._tree.Clear();

        var root = this.Details.CreateItem();

        //* Grab all available layers
        var layers = this._instance.GetLayers();
        for (int i = 0; i < layers.Length; i++)
        {
            var layerRoot = this.Details.CreateItem(root);
            var layer = layers[i];

            //* Setup the pots for it
            var firstSlot = this.Details.CreateItem(layerRoot);
            var secondSlot = this.Details.CreateItem(layerRoot);
            var thirdSlot = this.Details.CreateItem(layerRoot);
            var fourthSlot = this.Details.CreateItem(layerRoot);

            //* Pass the details
            layer.FirstSlot.SetupDetails(this.Details, firstSlot);
            layer.SecondSlot.SetupDetails(this.Details, secondSlot);
            layer.ThirdSlot.SetupDetails(this.Details, thirdSlot);
            layer.FourthSlot.SetupDetails(this.Details, fourthSlot);

            this._tree.Add(new()
            {
                root = layerRoot,
                firstSlot = firstSlot,
                secondSlot = secondSlot,
                thirdSlot = thirdSlot,
                fourthSlot = fourthSlot,
            });
        }

        this.UpdateDetails();
    }

    private void UpdateDetails()
    {
        var layers = this._instance.GetLayers();
        for (int i = 0; i < layers.Length; i++)
        {
            var isCurrent = this._focused.Y == i;
            var currSlot = this._focused.X;
            var treeLayer = this._tree[i];

            treeLayer.root.SetText(0, isCurrent ? $"Layer {i + 1} *" : $"Layer {i + 1}");
            treeLayer.firstSlot.SetText(0, isCurrent && currSlot == 0 ? "1st Slot *" : "1st Slot");
            treeLayer.secondSlot.SetText(0, isCurrent && currSlot == 1 ? "2nd Slot *" : "2nd Slot");
            treeLayer.thirdSlot.SetText(0, isCurrent && currSlot == 2 ? "3rd Slot *" : "3rd Slot");
            treeLayer.fourthSlot.SetText(0, isCurrent && currSlot == 3 ? "4th Slot *" : "4th Slot");
        }
    }

    private void OpenSeedStorage()
    {
        //* Ensure inventory list is empty
        this.List.Clear();

        //* Create a entry for each item
        this._seeds = this._instance.GetSeeds();
        foreach (var entry in this._seeds)
        {
            this.List.AddItem($"{entry.Seed.Name} (x{entry.Amount})", entry.Seed.Icon);
        }

        this._isStorageOpen = true;
    }

    private void OnInputChange(Vector2 input)
    {
        //* Should change what is focused here
        this._focused += input;
        this._focused = this._tower.ClampInput(this._focused);

        this.ProcessInput();
        this.UpdateDetails();
    }

    private void OnSeedActivated(long index)
    {
        if (!this._isStorageOpen) return;

        var selected = this._seeds[index];
        GD.Print($"Activated seed {selected.Seed.Name}({selected.Id})");

        var slot = this._tower.GetSlot(this._focused);
        slot.PlantSeed(this._instance.TakeSeed(selected.Id));
        this._isStorageOpen = false;
    }

    private void Close()
    {
        this._hasSetup = false;

        var layers = this._instance.GetLayers();
        for (int i = 0; i < layers.Length; i++)
        {
            var layer = layers[i];

            layer.FirstSlot.UIClose();
            layer.SecondSlot.UIClose();
            layer.ThirdSlot.UIClose();
            layer.FourthSlot.UIClose();
        }

        this._tower.UIClose();
        this._cursorState.SetBusy(false);
        this.QueueFree();
    }
}

public class LayerTree
{
    public TreeItem root;
    public TreeItem firstSlot;
    public TreeItem secondSlot;
    public TreeItem thirdSlot;
    public TreeItem fourthSlot;
}

public enum Tool
{
    WaterCan,
    Trowel
}
