namespace Crygotchi;

public class DialogueTextStep : DialogueStep
{
    public override DialogueStepType Type { get => DialogueStepType.Text; }
    public string Name;
    public string Text;
}
