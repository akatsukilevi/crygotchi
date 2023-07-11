using System.Collections.Generic;

namespace AfterlifeAdventures;

public partial class DialogueUI : Control
{
    [ExportCategory("References")]
    [Export] public Label CharacterName;
    [Export] public RichTextLabel Dialogue;

    private List<DialogueStep> _steps;
    private OSCController _osc;

    private bool _isSetup = false;
    private int _count = 0;

    public override void _Ready()
    {
        base._Ready();
        this._osc = this.GetNode<OSCController>("/root/OSCController");
    }

    public void Setup(List<DialogueStep> steps)
    {
        this._steps = steps;

        this.NextStep();
        this._isSetup = true;
    }

    private void ProcessInput()
    {
        this._osc.RegisterOSC(new OSC[]
        {
            new()
            {
                Name = "Next",
                Key = OSCKey.Primary,
                OnActivate = this.NextStep
            }
        });
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

        this.ProcessInput();
    }

    private void Finish()
    {
        this._isSetup = false;
        this.QueueFree();
    }
}
