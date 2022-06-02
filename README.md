# Ieg.Sequencer


## Configuration, build and run a sequence

Example configuration in .NET 6.0 style for the sequence of an OffTimer:

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

Build the sequence:

```c#
public class OffTimerExample
{
    public OffTimerExample() =>
        _sequence = SequenceBuilder.Build();
}
```

Run the sequence:

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


## Markdown guides 

Basic [^md_basic]
Extended [^md_extended]
Cheat Sheet [^md_cheat]



## Footnotes

[^md_basic]: https://www.markdownguide.org/basic-syntax
[^md_extended]: https://www.markdownguide.org/extended-syntax
[^md_cheat]:https://www.markdownguide.org/cheat-sheet/