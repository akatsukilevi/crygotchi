namespace Crygotchi;

public abstract class StateMachine
{
    protected IState _currentState;
    public void ChangeState(IState NextState)
    {
        _currentState?.Exit();
        _currentState = NextState;
        _currentState.Enter();
    }

    public void HandleInput()
    {
        _currentState.HandleInput();
    }

    public void Process()
    {
        _currentState.Process();
    }

    //I dislike this part, but people are always aruging about these things, so for now it sits.
    public IState GetCurrentState()
    {
        return _currentState;
    }
}
