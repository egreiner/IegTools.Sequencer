﻿namespace IegTools.Sequencer.Handler;

/// <summary>
/// The base handler
/// </summary>
public abstract class HandlerBase : IHandler
{
    /// <inheritdoc />
    public bool ResumeSequence { get; set; } = true;

    /// <summary>
    /// The constraint that should be met to make the transition
    /// </summary>
    public Func<bool> Condition { get; set; }

    /// <summary>
    /// The action that will be invoked when the state transition will be executed
    /// </summary>
    public Action Action        { get; set; }


    /// <inheritdoc />
    public abstract bool IsRegisteredState(string state);

    /// <inheritdoc />
    public abstract bool IsConditionFulfilled(ISequence sequence);

    /// <summary>
    /// Returns true if the handler condition is fulfilled
    /// </summary>
    protected bool IsConditionFulfilled() =>
        Condition?.Invoke() ?? true;


    /// <inheritdoc />
    public abstract void ExecuteAction(ISequence sequence);

    /// <inheritdoc />
    public bool ExecuteIfValid(ISequence sequence)
    {
        var complied = IsConditionFulfilled(sequence);
        if (complied)  ExecuteAction(sequence);

        return complied;
    }
}