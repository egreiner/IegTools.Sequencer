# Ieg.Sequencer


## Configure, build and run a sequence
### Simple configuration (.NET 6 style)

A simple example configuration for an OffTimer-sequence:

```c#
public class OffTimerExample
{
    private ISequenceBuilder SequenceBuilder =>
        SequenceBuilder.Create("Off")
            .AddForceState("On", () => LastValue)
            .AddTransition("On", "PrepareOff", () => !LastValue, () => Stopwatch.Restart())
            .AddTransition("PrepareOff", "Off", () => Stopwatch.Expired(MyTimeSpan));
}
```

### Build the sequence

```c#
public class OffTimerExample
{
    private ISequence _sequence;
	
    public OffTimerExample() =>
        _sequence = SequenceBuilder.Build();
}
```

### Run the sequence

```c#
public class OffTimerExample
{
    public OffTimerExample In(bool value)
    {
        LastValue = value;
        _sequence.Run();

        return this;
    }
}
```

### Complex configuration (.NET 5 style)

A more complex example configuration for a pump-anti-sticking-sequence:

```c#
 private ISequenceBuilder SequenceBuilder =>
        SequenceBuilder.Configure("Paused", builder =>
        {
            builder.AddForceState("Paused", () => !_onTimer.Out);
            builder.AddTransition("Paused", "Activated",
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

            builder.AddTransition("Pump off", "Paused",
                () => Stopwatch.Expired(_settings.PauseTime) && sequenceDone(),
                () => _onTimer.In(false));

            bool sequenceDone() => _countStarts > _settings.PumpStartQuantity;
        });
```


## Markdown guides 

Basic [^md_basic]
Extended [^md_extended]
Cheat Sheet [^md_cheat]



## Footnotes

[^md_basic]: https://www.markdownguide.org/basic-syntax
[^md_extended]: https://www.markdownguide.org/extended-syntax
[^md_cheat]:https://www.markdownguide.org/cheat-sheet/