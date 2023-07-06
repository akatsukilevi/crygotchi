namespace Crygotchi;

public interface IState
{
    public void Enter();
    public void Exit();
    public void Process();
    public void HandleInput();
}
