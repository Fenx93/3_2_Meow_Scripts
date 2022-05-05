using static GameplayController;

public class DelayedAction
{
    public DelayedDelegate Delegate { get; private set; }
    public int Value { get; private set; }

    public DelayedAction(DelayedDelegate delayedAction, int damage)
    {
        Delegate = delayedAction;
        Value = damage;
    }
}
