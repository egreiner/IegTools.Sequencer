namespace IegTools.Sequencer.Core;

using System;
using JetBrains.Annotations;

internal abstract class ChangeDetectorBase<TValue> : IChangeDetector<TValue>
{
    protected Func<TValue> DetectValueFunc { get; private set; }
    protected Action OnChangeAction        { get; private set; }
    protected bool ActionSuspended         { get; private set; }


    protected ChangeDetectorBase(string name) =>
        Name = name;


    public string Name { get; set; }

    public DateTime Timestamp   { get; private set; } = DateTime.Now;
    public TValue PreviousValue { get; private set; }
    
    public (TValue Value, TimeSpan Duration) LastState { get; private set; }
    

    public abstract void Detect(bool enable = true);
    

    public void ResumeAction()  => ActionSuspended = false;
    public void SuspendAction() => ActionSuspended = true;

    public void InvokeAction(bool enable = true)
    {
        if (enable) OnChangeAction?.Invoke();
    }

    
    /// <inheritdoc />
    public IChangeDetector<TValue> SetValue(TValue value)
    {
        PreviousValue = value;
        return this;
    }


    protected IChangeDetector<TValue> OnChange([NotNull] Func<TValue> detectValue, [NotNull] Action action)
    {
        DetectValueFunc = detectValue ?? throw new ArgumentNullException(nameof(detectValue));
        OnChangeAction  = action      ?? throw new ArgumentNullException(nameof(action));

        SaveLastState(DetectValueFunc.Invoke(), DateTime.Now);

        return this;
    }

    protected void SaveLastState(TValue value, DateTime timestamp)
    {
        LastState     = (PreviousValue, timestamp - Timestamp);
        PreviousValue = value;
        Timestamp     = timestamp;
    }
}