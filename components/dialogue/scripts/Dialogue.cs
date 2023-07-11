using System.Collections.Generic;

namespace AfterlifeAdventures;

public class Dialogue
{
    private List<DialogueStep> _steps = new();

    public Dialogue AddText(string name, string text)
    {
        this._steps.Add(new DialogueTextStep() { Name = name, Text = text });
        return this;
    }

    public Dialogue AddCallback(Action callback)
    {
        this._steps.Add(new DialogueCallbackStep() { Callback = callback });
        return this;
    }

    public List<DialogueStep> Build()
    {
        return this._steps;
    }
}

public abstract class DialogueStep
{
    public abstract DialogueStepType Type { get; }
}

public enum DialogueStepType
{
    Text = 0,
    Callback = 1
}
