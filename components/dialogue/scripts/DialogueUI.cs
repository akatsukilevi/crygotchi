using System.Collections.Generic;

namespace Crygotchi;

public partial class DialogueUI : Control
{
    [ExportCategory("References")]
    [Export] public Label CharacterName;
    [Export] public RichTextLabel Dialogue;

    private List<DialogueStep> _steps;

    private bool _isSetup = false;
    private int _count = 0;

    public void Setup(List<DialogueStep> steps)
    {
        this._steps = steps;

        this.NextStep();
        this._isSetup = true;
    }

    public override void _Process(double delta)
    {
        if (!this._isSetup) return;
        if (!Input.IsActionJustPressed("cursor_action_primary")) return;

        //* Should iterate to the next one
        this.NextStep();
    }

    private void NextStep()
    {
        if (this._steps.Count <= this._count)
        {
            this.Finish(); //* If past the list, means we're done
            return;
        }

        //* Grab the current step
        var step = this._steps[this._count];

        //* Advance to the next one in advance
        this._count += 1;

        //* Process accordingly
        switch (step.Type)
        {
            case DialogueStepType.Text:
                this.CharacterName.Text = (step as DialogueTextStep).Name;
                this.Dialogue.Text = (step as DialogueTextStep).Text;
                break;
            case DialogueStepType.Callback:
                (step as DialogueCallbackStep).Callback.Invoke();
                this.NextStep(); //* Will continue to the next one since actions have no dialogues
                break;
        }
    }

    private void Finish()
    {
        this._isSetup = false;
        this.QueueFree();
    }
}
