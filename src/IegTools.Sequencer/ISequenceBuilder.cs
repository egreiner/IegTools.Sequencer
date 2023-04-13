namespace IegTools.Sequencer;

using Handler;
using Validation;

public interface ISequenceBuilder
{
    /// <summary>
    /// The sequence configuration
    /// </summary>
    SequenceConfiguration Configuration { get; init; }

    /// <summary>
    /// Builds a default sequence with the specified configuration
    /// Throws an exception if the sequence is not valid.
    /// </summary>
    ISequence Build();

    /// <summary>
    /// Builds a customized sequence with the specified configuration
    /// Throws an exception if the sequence is not valid.
    /// </summary>
    ISequence Build<TSequence>() where TSequence : ISequence, new();

    /// <summary>
    /// Sets the initial state
    /// </summary>
    /// <param name="initialState">The initial state</param>
    ISequenceBuilder SetInitialState(string initialState);

    /// <summary>
    /// Adds an sequence handler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler">The handler.</param>
    ISequenceBuilder AddHandler<T>(T handler) where T: IHandler;

    /// <summary>
    /// Adds an Handler-Validator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    ISequenceBuilder AddValidator<T>() where T : IHandlerValidator, new();

    /// <summary>
    /// Does not validate the sequence configuration on build
    /// </summary>
    ISequenceBuilder DisableValidation();

    /// <summary>
    /// Does not validate states that are in this list
    /// </summary>
    /// <param name="states">A list of states that should not be validated.</param>
    ISequenceBuilder DisableValidationForStates(params string[] states);
}