namespace SuperScrabble.WebApi.Timers;

public abstract class GameTimer
{
    protected readonly System.Timers.Timer _timer = new();

    public virtual void Reset()
    {
        _timer.AutoReset = true;
        _timer.Interval = 1_000;
    }

    public virtual void Start()
    {
        _timer.Start();
    }

    public virtual void Dispose()
    {
        _timer.Dispose();
    }
}
