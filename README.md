# IegTools.Sequencer  

IegTools.Sequencer provides a fluent interface for creating easy-to-read and extensible sequences,
eliminating the need for lengthy if/else statements.  
The library is written in C# 11.0 and targets .NET Standard 2.0 (.NET Core and .NET Framework).

The library allows you to define:

- Transition jobs: from one state to another state, when it should be triggered, and the action that should be invoked.  
- Force state on specified conditions.  
- Invoke actions on specified states.  

### Build Status  
&nbsp; ![workflow tests](https://github.com/egreiner/IegTools.Sequencer/actions/workflows/ci-tests.yml/badge.svg)  
&nbsp; ![workflow complete](https://github.com/egreiner/IegTools.Sequencer/actions/workflows/create-nuget-package.yaml/badge.svg)    



# Table of Contents
[Installation](#installation)  
[Usage](#usage)  
[States](#states)  
[State Tags](#state-tags)  
[Validation](#validation)  
[Handler](#handler)  
[SequenceBuilder Extensions](#sequencebuilder-extensions)  
[Extensibility](#extensibility)  
[Version Changes](#version-changes)  
[Breaking Changes](#breaking-changes)  



# Installation  
The library is available as a [NuGet package](https://www.nuget.org/packages/IegTools.Sequencer/).  



# Usage
## Configure, build and run a sequence
### Configuration .NET 6 style

A simple example configuration and usage for an OnTimer-sequence:

``` c#
public class OnTimerExample
{
    private ISequence _sequence;
	
    public OnTimerExample() =>
        _sequence = SequenceConfig.Build();


    public void In(bool value)
    {
        LastValue = value;

        _sequence.Run();
    }

    private ISequenceBuilder SequenceConfig =>
        SequenceBuilder.Create()
            .AddForceState(">Off", () => !LastValue)
            .AddTransition(">Off", "PrepareOn", () => LastValue, () => _sequence.Stopwatch.Restart())
            .AddTransition("PrepareOn", "!On", () => _sequence.Stopwatch.Expired(MyTimeSpan));
}
```

[Top 🠉](#table-of-contents)


### Configuration .NET 5 style

A more complex example configuration for a pump-anti-sticking-sequence:

``` c#
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

[Top 🠉](#table-of-contents)


## Config in Detail

- Force state on condition:  
  `builder.AddForceState("ForceState", constraint)`  

- State transition on condition (with optional action)  
  `builder.AddTransition("FromState", "ToState", constraint, action)`  

- Action on state:  
  `builder.AddStateAction("State", action)` 

[Top 🠉](#table-of-contents)


# States

States can be defined as strings or enums, internally they will be stored as strings.

[Top 🠉](#table-of-contents)


# State Tags

State-Tags can only be used with string-states.
For enum-states there are other possibilities. (-> Validation Handler)  

There are available two state tags as prefix for states
- the IgnoreTag '!'
- and the InitialStateTag '>'

## IgnoreTag
Use the IgnoreTag as prefix for an state to tell the Validator not to check this state for counterpart-plausibility.

Example:  
``` C#
 .AddTransition("PrepareOff", "!Off", () => Stopwatch.Expired(MyTimeSpan));
```


## InitialStateTag
Use the InitialStateTag as prefix for an state to tell the Sequence what state to start from.

Example:  
``` C#
 builder.AddForceState(">Paused", () => !_onTimer.Out);
```

[Top 🠉](#table-of-contents)


# Validation

The sequence will be validated on build.  
`_sequence = builder.Build();` 


Validation Handler:

- InitialState must be defined
- The InitialState must have an counterpart in a StateTransition
- The Sequence must have at least two steps
- Each 'NextStep' must have a counterpart StateTransition with an matching 'CurrentState'
- Each 'CurrentState' must have a counterpart StateTransition with an matching 'NextStep' or ForceState

Validation could be disabled
- completely turn off validation  
    `builder.DisableValidation()`  

- or with specifying states that shouldn't be validated:  
    `builder.DisableValidationForStates("state1", "state2", ...)`  
    `builder.DisableValidationForStates(Enum.State1, Enum.State2, ...)`  

- or with the IgnoreTag '!':  
    `.AddTransition("PrepareOn", "!On", ...);`  

[Top 🠉](#table-of-contents)


# Handler

Internally the Framework is working with Handler (you can write your own customized handler).
The Handler describe what they are supposed to do within the sequence.

There are five default handler at the moment:  
- The [StateTransitionHandler](#statetransitionhandler)  
- The [ContainsStateTransitionHandler](#containsstatetransitionhandler)  
- The [AnyStateTransitionHandler](#anystatetransitionhandler)  
- The [ForceStateHandler](#forcestatehandler)  
- The [StateActionHandler](#stateactionhandler)    

## StateTransitionHandler
The StateTransitionHandler is responsible for the transition between two states.
It switches the sequence from start-state to end-state when the sequence current state is the start-state and the state-transition-condition is true.
Additionally an action can be executed when the transition is done.

## ContainsStateTransitionHandler
It's basically the same as the StateTransitionHandler, but it can handle multiple start-states to one end-state.

## AnyStateTransitionHandler
It's basically the same as the ContainsStateTransitionHandler, but it can handle all start-states that contains the specified string to one end-state.

## ForceStateHandler
Forces the sequence into the specified state when the force-state-condition is fulfilled.
Additionally an action can be executed when the force-transition is done.

## StateActionHandler
Executes continuously the specified action when the sequence is in the specified state.


[Top 🠉](#table-of-contents)  

# SequenceBuilder Extensions
## ExtensionMethods for existing Handler
All existing Handler can be added to a sequence via the SequenceBuilders ExtensionMethods.

## AllowOnceIn(timeSpan)
Each Transition can be enhanced with the ExtensionMethod .AllowOnceIn(timeSpan).
This prevents the transition from being triggered again within the specified timeSpan.

Example from an xUnit test:
``` C#
    [Fact]
    public void Test_AllowOnlyOnceIn_set()
    {
        var x = 0;
        var builder = SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("State1");
            builder.AddTransition("State1", "State2", () => true, () => x++)
                .AllowOnlyOnceIn(TimeSpan.FromSeconds(1))
                .DisableValidation();
        });

        var sut = builder.Build();

        sut.SetState("State1");

        for (int i = 0; i < 3; i++)
        {
            sut.Run();
            sut.SetState("State1");
        }

        x.Should().Be(1);
    }
```

For more examples take a look at the UnitTests.

[Top 🠉](#table-of-contents)



# Extensibility
Write your own customized
- Handler
- Sequence
- and Validator

TODO Documentation

[Top 🠉](#table-of-contents)


# Version Changes
## v2.1 -> v2.2
- new builder.SetLogger(...) methods

## v2.0 -> v2.1
- new ExtensionMethod .AllowOnceIn(timeSpan)

[Top 🠉](#table-of-contents)  


# Breaking Changes

so far none

[Top 🠉](#table-of-contents)  
