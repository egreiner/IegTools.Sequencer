# Ieg.Sequencer

A small framework to create sequences, simple to use, simple to extend.

# Usage
## Configure, build and run a sequence
### Configuration .NET 6 style

A simple example configuration for an OffTimer-sequence:

```c#
public class OffTimerExample
{
    private ISequenceBuilder SequenceConfig =>
        SequenceBuilder.Create()
            .AddForceState("On", () => LastValue)
            .AddTransition("On", "PrepareOff", () => !LastValue, () => Stopwatch.Restart())
            .AddTransition("PrepareOff", ">Off", () => Stopwatch.Expired(MyTimeSpan));
}
```

### Build the sequence

```c#
public class OffTimerExample
{
    private ISequence _sequence;
	
    public OffTimerExample() =>
        _sequence = SequenceConfig.Build();
}
```

### Run the sequence

The sequence will be executed in the confgured order 

```c#
public class OffTimerExample
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


## Descriptors

Internally the Framework is working with Descriptors (you can write your own customized descriptor).
The Descriptors describe what they are supposed to do within the sequence.

There are three default descriptor:
- The StateTransitionDescriptor
- The ForceStateDescriptor
- The StateActionDescriptor



## Validation

The sequence will be validated on build.

Validation Rules:

- InitialState must be defined
- The InitialState must have an counterpart in a StateTransition
- The Sequence must have at least two steps
- Each 'NextStep'' must have a counterpart StateTransition with an 'CurrentState'

Validation could be disabled completely with 
 ```C#
 builder.DisableValidation()
 ```
or with tagging states that shouldn't' be validated
 ```C#
 builder.DisableValidationForStates("state1", "state2", ...)
 ```



## State Tags

Ther are available two state tags as prefix for states
- the IgnoreTag '!'
- and the InitialStateTag '>'

### IgnoreTag
Use the IgnoreTag as prefix for an state to tell the Validator not to check this state for plausibility.

### InitialStateTag
Use the InitialStateTag as prefix for an state to tell the Sequence from what state to start from.



## Markdown guides 

Basic [^md_basic]
Extended [^md_extended]
Cheat Sheet [^md_cheat]



## Footnotes

[^md_basic]: https://www.markdownguide.org/basic-syntax
[^md_extended]: https://www.markdownguide.org/extended-syntax
[^md_cheat]:https://www.markdownguide.org/cheat-sheet/