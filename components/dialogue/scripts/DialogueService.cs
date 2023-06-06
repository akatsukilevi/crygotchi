using System.Collections.Generic;

namespace Crygotchi;

public partial class DialogueService : Node2D
{
    private PackedScene _dialogueTemplate;

    public override void _Ready()
    {
        base._Ready();

        this._dialogueTemplate = GD.Load<PackedScene>("res://components/dialogue/dialogue_ui.tscn");
    }

    public void RunDialogue(List<DialogueStep> steps)
    {
        //* Should run it
        var root = this.GetTree().Root;

        var dialogueUI = this._dialogueTemplate.Instantiate<DialogueUI>();
        dialogueUI.Ready += () => dialogueUI.Setup(steps);
        root.AddChild(dialogueUI);
    }
}
