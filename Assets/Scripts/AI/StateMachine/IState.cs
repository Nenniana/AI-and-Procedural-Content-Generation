
public interface IState<T>
{
    abstract void Enter();
    abstract void Execute();
    abstract void ExecuteFixed();
    abstract void Exit();
}
