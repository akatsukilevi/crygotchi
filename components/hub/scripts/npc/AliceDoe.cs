namespace AfterlifeAdventures;

public partial class AliceDoe : NPC
{
    private DialogueService _dialogue;

    public override void _Ready()
    {
        base._Ready();
        this._dialogue = this.GetNode<DialogueService>("/root/DialogueService");
    }

    protected override void StartInteraction()
    {
        var dialog = new Dialogue()
            .AddText("Alice_", "Hello! My name is Alice_!")
            .AddText("AkatsukiLevi (Developer)", "Nah just joking")
            .AddText("AkatsukiLevi (Developer)", "This game is still heavily early in development")
            .AddText("AkatsukiLevi (Developer)", "There isn't much to do yet, but progress is steadily going!")
            .AddText("AkatsukiLevi (Developer)", "Hope you enjoy this really small demo preview version!")
            .AddText("AkatsukiLevi (Developer)", "Hopefully the next demo versions will come as planned!")
            .AddCallback(() => this.FinishInteraction())
            .Build();

        this._dialogue.RunDialogue(dialog);
    }
}
