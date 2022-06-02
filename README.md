# Ieg.Sequencer


## Configuration, build and run a sequence

Example configuration in .NET 6.0 style for the sequence for an OffTimer:

```c#
private ISequenceBuilder SequenceConfig =>
    SequenceBuilder.Create("Off")
        .AddForceState("On", () => LastValue)
        .AddTransition("On", "PrepareOff", () => !LastValue, () => Stopwatch.Restart())
        .AddTransition("PrepareOff", "Off", () => Stopwatch.Expired(MyTimeSpan));
```

Build the sequence:

```c#
public OffTimerExample() =>
    _sequence = SequenceBuilder.Build();
```

Run the sequence:

```c#
_sequence.Run();
```


## Markdown guides 

Basic [^md_basic]
Extended [^md_extended]
Cheat Sheet [^md_cheat]



## Footnotes

[^md_basic]: https://www.markdownguide.org/basic-syntax
[^md_extended]: https://www.markdownguide.org/extended-syntax
[^md_cheat]:https://www.markdownguide.org/cheat-sheet/