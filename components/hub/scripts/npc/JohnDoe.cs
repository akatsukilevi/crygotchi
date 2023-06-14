namespace Crygotchi;

public partial class JohnDoe : NPC
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
            .AddText("John Doe", "Hello! My name is John Doe!")
            .AddText("John Doe", "Theses texts will show only one by one")
            .AddCallback(() => GD.Print("Hello from callback!"))
            .AddText("John Doe", "The previous one triggered a callback! And next one will finish it")
            .AddCallback(() => this.FinishInteraction())
            .Build();

        this._dialogue.RunDialogue(dialog);
    }
}
