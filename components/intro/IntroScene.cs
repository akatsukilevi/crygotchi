namespace Crygotchi;

public partial class IntroScene : Node
{
    [ExportCategory("References")]
    [Export] public PackedScene MainScene;

    private Node _mainMenuScene;

    public override void _Ready()
    {
        base._Ready();

        this._mainMenuScene = this.MainScene.Instantiate();
    }

    private void SwitchToMainMenu()
    {
        this.GetTree().Root.AddChild(this._mainMenuScene);
    }
}
