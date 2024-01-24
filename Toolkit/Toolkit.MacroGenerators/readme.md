# Sequential Macro Generator
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.PDDLSharp.Toolkit.MacroGenerators-orange)

PDDLSharp also have a simple sequential macro generator. It can generate lifted macros based on reoccuring sequences in [ActionPlan](../../Models/FastDownward/Plans/ActionPlan.cs)s.
It works as follows
1. Find all the possible sequence combinations possible across all the given plans.
2. Sort all sequences by occurence.
3. Remove all sequences where there is no inner-entanglements (e.g. `pick ball1 rooma left`, `pick ball2 rooma right`).
4. Foreach remaining sequence, combine the actions into a macro.
5. Return found macros.

This results in macros that can look like this (for the gripper domain):
```pddl
(:action pick-pick-move-drop-drop
  :parameters (?ball1 ?ball2 ?rooma ?roomb ?left ?right)
  :precondition  
    (and 
      (free ?left) (free ?right)
      (ball ?ball1) (ball ?ball2)
      (room ?rooma) (room ?roomb)
      (gripper ?left) (gripper ?right)
      (at-robby ?rooma) 
      (at ?ball1 ?rooma)
      (at ?ball2 ?rooma)
    )
    :effect 
    (and 
      (at-robby ?roomb)
      (not (at-robby ?rooma))
      (not (at ?ball1 ?rooma))
      (not (at ?ball2 ?rooma))
      (at ?ball1 ?roomb)
      (at ?ball2 ?roomb)
    )
)
```
That effectively takes two balls, moves to the other room and drops them both, consisting of 5 actions in total.

#### Examples
To find all macros for a given set plans and a domain:
```csharp
PDDLDecl decl = new PDDLDecl(...)
IMacroGenerator<List<ActionPlan>> generator = new SequentialMacroGenerator(decl);
List<ActionPlan> plans = new List<ActionPlan>(...);
List<ActionDecl> macros = generator.FindMacros(plans);
```

To find top 10 macros for a given set of plans and a domain:
```csharp
PDDLDecl decl = new PDDLDecl(...)
IMacroGenerator<List<ActionPlan>> generator = new SequentialMacroGenerator(decl);
List<ActionPlan> plans = new List<ActionPlan>(...);
List<ActionDecl> macros = generator.FindMacros(plans, 10);
```
