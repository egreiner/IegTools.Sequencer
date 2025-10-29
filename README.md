# IegTools.Sequencer  

IegTools.Sequencer is the predecessor of [FluentSeq](https://www.nuget.org/packages/FluentSeq/1.3.0).

IegTools.Sequencer provides a fluent interface for creating easy-to-read and extensible sequences,
eliminating the need for lengthy if-else statements.  
The library is written in C# 12.0 and targets .NET Standard 2.0 (.NET and .NET Framework).

The library allows you to define:

- Various transition jobs: from one state to another state, when it should be triggered and an optional action that should be invoked.    
- Force state on specified conditions.  
- Invoke actions on specified states.  
- Activate Debug Logging.  

The Test coverage for .NET 8.0 is greater than 95%


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
[Version Changes](#version-changes)  
[Breaking Changes](#breaking-changes)  
[Preview next Version v4.0](#preview-next-version-v4)  


# Installation
The library is available as a [NuGet package](https://www.nuget.org/packages/IegTools.Sequencer/).  



# Usage
## Configure, build and run a sequence
### Create a sequence in a compact style

A simple example configuration and usage for an OnTimer-sequence:

``` c#
public class OnTimerExample
{
    private readonly ISequence _sequence;
    private readonly DefaultSequenceStates _state = new();
	
    public OnTimerExample() =>
        _sequence = SequenceConfig.Build();


    public void In(bool value)
    {
        LastValue = value;
        _sequence.Run();
    }

    private ISequenceBuilder SequenceConfig =>
        SequenceBuilder.Create()
            .SetInitialState(_state.Off)
            .DisableValidationForStates(_state.On)
            .AddForceState(_state.Off,                () => !LastValue)
            .AddTransition(_state.Off, _state.WaitOn, () => LastValue, () => _sequence.Stopwatch.Restart())
            .AddTransition(_state.WaitOn, _state.On,  () => _sequence.Stopwatch.IsExpired(OnDelay));
}
```

[Top 🠉](#table-of-contents)


### Configure a sequence in a detailed style

A more complex example configuration for a pump-anti-sticking-sequence:

``` c#
 private ISequenceBuilder SequenceConfig =>
        SequenceBuilder.Configure(builder =>
        {
            builder.SetInitialState("Paused")

            builder.AddForceState("Paused", () => !_onTimer.Out);
            
            builder.AddTransition("Paused", "Activated",
                () => _onTimer.Out,
                () => _countStarts = 1);
            
            builder.AddTransition("Activated", "Pump on",
                () => true,
                () => Stopwatch.Restart());
            
            builder.AddTransition("Pump on", "Pump off",
                () => Stopwatch.IsExpired(_settings.RunTime * _countStarts.Factorial()),
                () =>
                {
                    Stopwatch.Restart();
                    _countStarts++;
                });

            builder.AddTransition("Pump off", "Pump on",
                () => Stopwatch.Expired(_settings.PauseTime) && !sequenceDone());

            builder.AddTransition("Pump off", "Paused",
                () => Stopwatch.IsExpired(_settings.PauseTime) && sequenceDone(),
                () => _onTimer.In(false));

            bool sequenceDone() => _countStarts > _settings.PumpStartQuantity;
        });
```

[Top 🠉](#table-of-contents)


## Configurations in Detail

- State transition on condition (with optional action)  
  Executes a sequence state transition from one state to another state when the condition is true.   
  `builder.AddTransition("FromState", "ToState", condition, action)`
  

- Any State transition on condition (with optional action)  
  Executes a sequence state transition from multiple states to another state when the condition is true.   
  `string[] currentStateContains = { "State1", "State2" , "StateX" };`    
  `builder.AddAnyTransition(currentStateContains, "ToState", condition, action)`  
  

- Contains State transition on condition (with optional action)  
  Executes a sequence state transition from states that contains the specified string to another state when the condition is true.   
  `builder.AddContainsTransition("FromStateContains", "ToState", condition, action)`  
  

- Force state on condition:  
  `builder.AddForceState("ForceState", condition)`
  

- Action on state:  
  `builder.AddStateAction("State", action)` 

[Top 🠉](#table-of-contents)


# States

States can be defined as strings or enums, internally they will be stored as strings.

[Top 🠉](#table-of-contents)




# State Tags

[This feature will be deleted in the next major version (v4.0). 
The existing validators will detect if you have done the necessary changes
and throw an exception if not.
(the Initial-State and the ignored States should be defined explicitly)]

State-Tags can only be used with string-states.
For enum-states there are other possibilities. (-> Validation Handler)  

There are available two state tags as prefix for states
- the IgnoreTag '!'
- and the InitialStateTag '>'


## IgnoreTag
Use the IgnoreTag as prefix for a state to tell the Validator not to check this state for counterpart-plausibility.

Example:  
``` C#
 .AddTransition("PrepareOff", "!Off", () => Stopwatch.Expired(MyTimeSpan));
```


## InitialStateTag
Use the InitialStateTag as prefix for a state to tell the Sequence what state to start from.

Example:  
``` C#
 builder.AddForceState(">Paused", () => !_onTimer.Out);
```

[This feature will be deleted in the next major version (v4.0).
The existing validators will detect if you have done the necessary changes
and throw an exception if not.
(the Initial-State and the ignored States should be defined explicitly)]

[Top 🠉](#table-of-contents)




# Validation

The sequence will be validated on build.  
`_sequence = builder.Build();` 


Validation Handler:

- InitialState must be defined
- The InitialState must have a counterpart in a StateTransition
- The Sequence must have at least two steps
- Each 'NextStep' must have a counterpart StateTransition with a matching 'CurrentState'
- Each 'CurrentState' must have a counterpart StateTransition with a matching 'NextStep' or ForceState

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

Internally the Framework is working with Handler.  
The Handler describe what they are supposed to do within the sequence.  

There are five handler at the moment:    
- The [StateTransitionHandler](#statetransitionhandler)  
- The [ContainsStateTransitionHandler](#containsstatetransitionhandler)  
- The [AnyStateTransitionHandler](#anystatetransitionhandler)  
- The [ForceStateHandler](#forcestatehandler)  
- The [StateActionHandler](#stateactionhandler)    

## StateTransitionHandler
The StateTransitionHandler is responsible for the transition between two states.  
It switches the sequence from start-state to end-state when the sequence current state is the start-state and the state-transition-condition is true.  
Additionally, an action can be executed when the transition is done.  

## ContainsStateTransitionHandler
It's basically the same as the StateTransitionHandler, but it can handle multiple start-states to one end-state.  

## AnyStateTransitionHandler
It's basically the same as the ContainsStateTransitionHandler, but it can handle all start-states that contains the specified string to one end-state.  

## ForceStateHandler
Forces the sequence into the specified state when the force-state-condition is fulfilled.  
Additionally, an action can be executed when the force-transition is done.  

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



# Version Changes
## v2.2 -> v3.0
- new DefaultSequenceStates, a set of standard (string) states for a sequence  
- new builder.SetOnStateChangedAction(...);  
- changed builder.SetLogger(...) to builder.ActivateDebugLogging(...)  
- update NuGet packages  
- bunch of internal changes  


## v2.1 -> v2.2
- new builder.SetLogger(...) methods  

## v2.0 -> v2.1
- new ExtensionMethod .AllowOnceIn(timeSpan)  

[Top 🠉](#table-of-contents)  


# Breaking Changes
## v2.2 -> v3.0
- changed builder.SetLogger(...) to builder.ActivateDebugLogging(...)  

[Top 🠉](#table-of-contents)  



# Preview next Version v4

Removing the State-Tags (InitialStateTag and IgnoreTag) [State Tags 🠉](#state-tags)  

Thinking about:  
Renaming sequence.HasCurrentState(state) to sequence.IsInState(state)  
Renaming sequence.HasAnyCurrentState(states) to sequence.IsInStates(state) or IsInAnyState(states)  

Removing FluentAssertions from UnitTests because of there new license since v8.0.0 (and costs about $130,- per dev)
 - maybe moving to another assertion library (e.g. Shouldly, ...)

[Top 🠉](#table-of-contents)  
