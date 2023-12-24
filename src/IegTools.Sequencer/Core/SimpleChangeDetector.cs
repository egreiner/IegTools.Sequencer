namespace IegTools.Sequencer.Core;

using System;
using JetBrains.Annotations;

/// <summary>
/// Favor this SimpleChangeDetector over the ChangeDetector
/// </summary>
internal class SimpleChangeDetector<TValue> : ChangeDetectorBase<TValue>
{
    private readonly object _lockObject = new();


    public SimpleChangeDetector(string name) : base(name) { }


    public override void Detect(bool enable = true)
    {
        if (!enable) return;

        lock (_lockObject)
        {
            var value = DetectValueFunc != null ? DetectValueFunc.Invoke() : default;
            var hasChanged = (PreviousValue == null && value != null) || (!PreviousValue?.Equals(value) ?? false);

            if (!hasChanged) return;

            SaveLastState(value, DateTime.Now);
            InvokeAction(!ActionSuspended);
        }
    }

    public new IChangeDetector<TValue> OnChange([NotNull] Func<TValue> detectValue, [NotNull] Action action) =>
        base.OnChange(detectValue, action);
}