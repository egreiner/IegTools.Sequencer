# IegTools.Sequencer in a Nutshell

IegTools.Sequencer provides a fluent interface for creating easy-to-read and extensible sequences,
eliminating the need for lengthy if/else statements.

Define transition jobs:   
from state -> to state, when it should be triggerd and the action that should be invoked

Force state on specified conditions

Invoke Actions on specified states


# Usage
## Configure, build and run a sequence
### Configuration .NET 6 style

A simple example configuration for an OnTimer-sequence:

```c#
public class OnTimerExample
{
    private ISequenceBuilder SequenceConfig =>
        SequenceBuilder.Create()
            .AddForceState(">Off", () => !LastValue)
            .AddTransition(">Off", "PrepareOn", () => LastValue, () => _sequence.Stopwatch.Restart())
            .AddTransition("PrepareOn", "!On", () => _sequence.Stopwatch.Expired(MyTimeSpan));
```

### Build the sequence

```c#
public class OnTimerExample
{
    private ISequence _sequence;
	
    public OnTimerExample() =>
        _sequence = SequenceConfig.Build();
}
```

### Run the sequence

The sequence will be executed in the configuration order .

```c#
public class OnTimerExample
{
    public void In(bool value)
    {
        LastValue = value;
        _sequence.Run();
        // or await _sequence.RunAsync();
    }
}
```

### Configuration .NET 5 style

A more complex example configuration for a pump-anti-sticking-sequence:

```c#
 private ISequenceBuilder SequenceConfig =>
        SequenceBuilder.Configure(builder =>
        {
            builder.AddForceState(">Paused", () => !_onTimer.Out);
            
            builder.AddTransition(">Paused", "Activated",
                () => _onTimer.Out,
                () => _countStarts = 1);
            
            builder.AddTransition("Activated", "Pump on",
                () => true,
                () => Stopwatch.Restart());
            
            builder.AddTransition("Pump on", "Pump off",
                () => Stopwatch.Expired(_settings.RunTime * _countStarts.Factorial()),
                () =>
                {
                    Stopwatch.Restart();
                    _countStarts++;
                });

            builder.AddTransition("Pump off", "Pump on",
                () => Stopwatch.Expired(_settings.PauseTime) && !sequenceDone());

            builder.AddTransition("Pump off", ">Paused",
                () => Stopwatch.Expired(_settings.PauseTime) && sequenceDone(),
                () => _onTimer.In(false));

            bool sequenceDone() => _countStarts > _settings.PumpStartQuantity;
        });
```

## Config in Detail

- Force state on condition:
  builder.AddForceState("ForceState", constraint)

- State transition on condition (with optional action)
  builder.AddTransition("FromState", "ToState", constraint, action)

- Action on state:
  builder.AddStateAction("State", action)

TODO Documentation



## States

States can be defined as strings or enums, internally they will be stored as strings.



## State Tags

State-Tags can only be used with string-states.
For enum-states there are other possibilities. (link to ...)

There are available two state tags as prefix for states
- the IgnoreTag '!'
- and the InitialStateTag '>'

### IgnoreTag
Use the IgnoreTag as prefix for an state to tell the Validator not to check this state for counterpart-plausibility.

Example:
```C#
 .AddTransition("PrepareOff", "!Off", () => Stopwatch.Expired(MyTimeSpan));
```


### InitialStateTag
Use the InitialStateTag as prefix for an state to tell the Sequence what state to start from.

Example:
```C#
 builder.AddForceState(">Paused", () => !_onTimer.Out);
```



## Validation

The sequence will be validated when build.
        _sequence = builder.Build();


Validation Rules:

- InitialState must be defined
- The InitialState must have an counterpart in a StateTransition
- The Sequence must have at least two steps
- Each 'NextStep' must have a counterpart StateTransition with an matching 'CurrentState'
- Each 'CurrentState' must have a counterpart StateTransition with an matching 'NextStep' or ForceState

Validation could be disabled completely:
 ```C#
 builder.DisableValidation()
 ```
or with specifing states that shouldn't be validated:
 ```C#
 builder.DisableValidationForStates("state1", "state2", ...)
 ```
or with the IgnoreTag:
TODO Documentation



## Extensibility
Write your own customized 
- Rules
- Sequence
- and Validator

TODO Documentation



### Rules

Internally the Framework is working with Rules (you can write your own customized rules).
The Rules describe what they are supposed to do within the sequence.

There are five default rule:
- The StateTransitionRule
- The ContainsStateTransitionRule
- The AnyStateTransitionRule
- The ForceStateRule
- The StateActionRule

TODO Documentation
