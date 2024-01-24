# State Space Simulator
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.Toolkit.Simulators-orange)

There is a State Space Simulator included with PDDLSharp.
This is a simulator that is capable of simulating the state changes for each action execution.
For each action the simulator is executing it:
1. Verifies if the precondition for the action is true
2. Executes the effects
If there are invalid arguments or type issues, the simulator will throw an exception.
This simulator also supports some of the more exotic node types, such as [DerivedPredicateExp](../../Models/PDDL/Expressions/DerivedPredicateExp.cs), [ExistsExp](../../Models/PDDL/Expressions/ExistsExp.cs), [ImplyExp](../../Models/PDDL/Expressions/ImplyExp.cs) etc.

#### Examples
Example of how to execute a grounded action with no arguments:
```csharp
PDDLDecl declaration = new PDDLDecl(...);
IStateSpaceSimulator simulator = new StateSpaceSimulator(declaration);
simulator.Step("actionName");
```
Example of how to execute a grounded action with two arguments:
```csharp
PDDLDecl declaration = new PDDLDecl(...);
IStateSpaceSimulator simulator = new StateSpaceSimulator(declaration);
simulator.Step("actionName", "obj1", "obj2");
```
You can keep adding as many arguments as the action needs:
```csharp
PDDLDecl declaration = new PDDLDecl(...);
IStateSpaceSimulator simulator = new StateSpaceSimulator(declaration);
simulator.Step("actionName", "obj1", "obj2", "obj3", "obj4", ...);
```
