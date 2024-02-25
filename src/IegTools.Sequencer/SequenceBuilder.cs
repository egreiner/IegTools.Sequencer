namespace IegTools.Sequencer;

using Microsoft.Extensions.Logging;
using FluentValidation;
using Handler;
using Logging;
using Validation;

#nullable enable

/// <summary>
/// Provides methods to build a sequence.
/// </summary>
public class SequenceBuilder : ISequenceBuilder
{
    private readonly IValidator<SequenceBuilder> _validator;
    private          ILoggerAdapter?             _debugLogger;
    private          bool                        _useDefaultValidators = true;


    private SequenceBuilder(IValidator<SequenceBuilder> validator) =>
        _validator = validator;


    /// <inheritdoc />
    public SequenceConfiguration Configuration { get; init; } = new();

    /// <inheritdoc />
    public SequenceData Data { get; } = new();

    /// <inheritdoc />
    public string DefaultDescription { get; } = "no description available";
    
    /// <inheritdoc />
    public ISequenceBuilder ActivateDebugLogging(ILogger? logger, EventId eventId, Func<IDisposable>? loggerScope = null)
    {
        DebugLoggingActivated = true;
        _debugLogger          = new LoggerAdapter(logger, eventId, loggerScope);
        return this;
    }

    /// <summary>
    /// The debug logging is activated
    /// </summary>
    public bool DebugLoggingActivated { get; private set; }


    /// <summary>
    /// Creates a new Sequence-Builder for configuration in .NET 6 style.
    /// This is good for short crispy configs.
    /// </summary>
    public static ISequenceBuilder Create() =>
        Create(new SequenceConfigurationValidator());

    /// <summary>
    /// Creates a new Sequence-Builder for configuration in .NET 6 style.
    /// This is good for short crispy configs.
    /// </summary>
    public static ISequenceBuilder Create(IValidator<SequenceBuilder> validator) =>
        new SequenceBuilder(validator);

    /// <summary>
    /// Creates a new Sequence-Builder with an empty sequence.
    /// In some scenarios it is useful to have an empty sequence.
    /// Validation is disabled.
    /// </summary>
    public static ISequenceBuilder CreateEmpty(string fixedState = "Empty")
    {
        var builder = Create(new SequenceConfigurationValidator())
            .DisableValidation()
            .SetInitialState(fixedState);
        builder.Configuration.IsEmpty = true;
        return builder;
    }


    /// <summary>
    /// Configures the sequence in .NET 5 style.
    /// This is good for larger complex configs.
    /// </summary>
    /// <param name="configurationActions">The action.</param>
    public static ISequenceBuilder Configure(Action<ISequenceBuilder> configurationActions)
    {
        var sequenceBuilder = Create();
        configurationActions.Invoke(sequenceBuilder);
        return sequenceBuilder;
    }

    /// <summary>
    /// Configures the sequence in .NET 5 style.
    /// This is good for larger complex configs.
    /// </summary>
    /// <param name="validator">Custom validator</param>
    /// <param name="configurationActions">The action.</param>
    public static ISequenceBuilder Configure(IValidator<SequenceBuilder> validator, Action<ISequenceBuilder> configurationActions)
    {
        var sequenceBuilder = Create(validator);
        configurationActions.Invoke(sequenceBuilder);
        return sequenceBuilder;
    }


    /// <inheritdoc />
    public ISequence Build() => Build<Sequence>();


    /// <inheritdoc />
    public ISequence Build<TSequence>() where TSequence : ISequence, new()
    {
        if (_useDefaultValidators)
            AddDefaultValidators();

        return BuildFinal<TSequence>();
    }


    private ISequence BuildFinal<TSequence>() where TSequence : ISequence, new()
    {
        if (!Configuration.DisableValidation)
            _validator?.ValidateAndThrow(this);

        return CreateSequence<TSequence>();
    }

    private ISequence CreateSequence<TSequence>() where TSequence : ISequence, new()
    {
        _debugLogger ??= new LoggerAdapter();
        var sequence = new TSequence().SetConfiguration(Configuration, Data);
        sequence.Logger = _debugLogger;

        foreach (var handler in Data.Handler)
        {
            handler.SetSequence(sequence);
            handler.Logger = _debugLogger;
        }

        return sequence;
    }



    /// <inheritdoc />
    public ISequenceBuilder AddHandler<T>(T handler) where T : IHandler
    {
        Data.Handler.Add(handler);
        return this;
    }


    /// <inheritdoc />
    public ISequenceBuilder AddValidator<T>() where T : IHandlerValidator, new()
    {
        Data.Validators.Add(new T());
        return this;
    }


    /// <inheritdoc />
    public ISequenceBuilder DisableValidation()
    {
        Configuration.DisableValidation = true;
        return this;
    }

    /// <inheritdoc />
    public ISequenceBuilder DisableValidationForStates(params string[] states)
    {
        Configuration.DisableValidationForStates = states;
        return this;
    }

    /// <inheritdoc />
    public ISequenceBuilder SetInitialState(string initialState)
    {
        Configuration.InitialState = initialState;
        return this;
    }

    /// <inheritdoc />
    public ISequenceBuilder SetOnStateChangedAction(Action onStateChangedAction) =>
        SetOnStateChangedAction(onStateChangedAction, () => true);

    /// <inheritdoc />
    public ISequenceBuilder SetOnStateChangedAction(Action onStateChangedAction, Func<bool> enabledFunc)
    {
        Data.OnStateChangedAction = (onStateChangedAction, enabledFunc);
        return this;
    }

    /// <inheritdoc />
    public ISequenceBuilder WithValidator<TValidator>() where TValidator : IHandlerValidator, new()
    {
        _useDefaultValidators = false;
        AddValidator<TValidator>();
        return this;
    }


    private void AddDefaultValidators()
    {
        AddValidator<DebugLoggingValidator>();
        AddValidator<InitialStateValidator>();
        AddValidator<ForceStateValidator>();
        AddValidator<StateTransitionValidator>();
        AddValidator<AnyStateTransitionValidator>();
        AddValidator<ContainsStateTransitionValidator>();
    }
}