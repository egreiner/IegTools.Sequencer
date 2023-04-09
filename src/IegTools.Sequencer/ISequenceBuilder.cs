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
    /// </summary>
    ISequence Build();

    /// <summary>
    /// Builds a customized sequence with the specified configuration
    /// </summary>
    ISequence Build<TSequence>() where TSequence : ISequence, new();

    /// <summary>
    /// Sets the initial state
    /// </summary>
    /// <param name="initialState">The initial state</param>
    ISequenceBuilder SetInitialState(string initialState);

    /// <summary>
    /// Adds an sequence rule
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rule">The rule.</param>
    ISequenceBuilder AddRule<T>(T rule) where T: IHandler;

    /// <summary>
    /// Adds an Rule-Validator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    ISequenceBuilder AddRuleValidator<T>() where T : ISequenceRuleValidator, new();

    /// <summary>
    /// Does not validate the sequence configuration on build
    /// </summary>
    ISequenceBuilder DisableValidation();

    /// <summary>
    /// Does not validate statuses that are in this list
    /// </summary>
    /// <param name="statuses">A list of statuses that should not be validated.</param>
    ISequenceBuilder DisableValidationForStates(params string[] statuses);
}