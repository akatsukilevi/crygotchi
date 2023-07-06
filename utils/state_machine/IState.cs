namespace Crygotchi;

public interface IState
{
    public void Enter();
    public void Exit();
    public void Process(double delta);
    public void PhysicsProcess(double delta);
}
