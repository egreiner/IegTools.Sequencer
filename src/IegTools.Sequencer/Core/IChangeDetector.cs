namespace IegTools.Sequencer.Core;

using System;

internal interface IChangeDetector<TValue>
{
    string Name          { get; set; }

    DateTime Timestamp   { get; }
    TValue PreviousValue { get; }
    
    void Detect(bool enable = true);

    void ResumeAction();
    void SuspendAction();
    void InvokeAction(bool enable = true);

    /// <summary>
    /// Use this after OnChange(...)
    /// </summary>
    IChangeDetector<TValue> SetValue(TValue value);
}