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
[Extensibility](#extensibility)  
[Handler](#handler)  



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


# Extensibility
Write your own customized 
- Handler
- Sequence
- and Validator

TODO Documentation


[Top 🠉](#table-of-contents)


## Handler

Internally the Framework is working with Handler (you can write your own customized handler).
The Handler describe what they are supposed to do within the sequence.

There are five default handler at the moment:  
- The StateTransitionHandler
- The ContainsStateTransitionHandler
- The AnyStateTransitionHandler
- The ForceStateHandler
- The StateActionHandler

TODO Documentation


[Top 🠉](#table-of-contents)  
