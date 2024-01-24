PDDLSharp includes a set of state spaces, that is able to "simulate" a state space for a given format.

# PDDL State Space
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.StateSpaces.PDDL-orange)

The PDDL State Space takes in a [PDDLDecl](../Models/PDDL/PDDLDecl.cs) and then allows you to execute entire PDDL nodes (such as an `ActionDecl`).
You can also check if a given node is "valid" in the current state space, as well as if the state space is in its goal state.

#### Example
Example of executing the first action in a PDDLDecl, if its preconditions are true.
```csharp
PDDLDecl decl = new PDDLDecl(...);
IPDDLState state = new PDDLStateSpace(decl);
if (state.IsNodeTrue(decl.Actions[0].Preconditions))
    state.ExecuteNode(decl.Actions[0].Effects);
```

# SAS State Space
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.StateSpaces.SAS-orange)

There is also a simulator for the [SASDecl](../Models/SAS/SASDecl.cs) that works on executing operators and their associated facts.

There is also a "relaxed" version of the SAS state space, where the `ExecuteNode` ignores the delete list of operators.

#### Example
Example of executing the first operator in a SASDecl, if its preconditions are true
```csharp
SASDecl decl = new SASDecl(...);
ISASState state = new SASStateSpace(decl);
if (state.IsNodeTrue(decl.Operators[0]))
    state.ExecuteNode(decl.Operators[0]);
```