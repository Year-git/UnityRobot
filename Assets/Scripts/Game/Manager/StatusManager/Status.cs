public abstract class Status
{
    public abstract void OnEnter();
    public abstract void OnExecute();
    public abstract void OnLateExecute();
    public abstract void OnFixedExecute();
    public abstract void OnExit();
}