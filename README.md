# Ieg.Sequencer in a Nutshell

Ieg.Sequencer allows you to create sequences in code, simple to use and simple to extend.  
It helps you get rid of long unreadable if/else statements that represent a sequence of events.


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



## Statuses

Statuses can be defined as strings or enums, internally they will be stored as strings.



## State Tags

State-Tags can only be used with string-statuses.
For enum-statuses there are other possibilities. (link to ...)

There are available two state tags as prefix for statuses
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
or with specifing statuses that shouldn't be validated:
 ```C#
 builder.DisableValidationForStatuses("state1", "state2", ...)
 ```
or with the IgnoreTag:
TODO Documentation



## Extensibility
Write your own customized 
- Descriptors
- Sequence
- and Validator

TODO Documentation



### Descriptors

Internally the Framework is working with Descriptors (you can write your own customized descriptor).
The Descriptors describe what they are supposed to do within the sequence.

There are three default descriptor:
- The StateTransitionDescriptor
- The ForceStateDescriptor
- The StateActionDescriptor

TODO Documentation

