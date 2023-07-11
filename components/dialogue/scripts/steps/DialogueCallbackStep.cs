namespace AfterlifeAdventures;

public class DialogueCallbackStep : DialogueStep
{
    public override DialogueStepType Type { get => DialogueStepType.Callback; }
    public Action Callback;
}
