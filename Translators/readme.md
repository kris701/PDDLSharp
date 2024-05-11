Translators are capable of taking in one format, and transform it into another.

## Grounders
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.PDDLSharp.Translators.Grounders-orange)

There are two grounders in PDDLSharp available. One for [IParametized](../Models/PDDL/IParametized.cs) (e.g. [ActionDecl](../Models/PDDL/Domain/ActionDecl.cs), [ForAllExp](../Models/PDDL/Expressions/ForAllExp.cs), [ExistsExp](../Models/PDDL/Expressions/ExistsExp.cs), etc.) nodes, one for [PredicateExp](../Models/PDDL/Expressions/PredicateExp.cs) nodes and one for [ActionDecl](../Models/PDDL/Domain/ActionDecl.cs) nodes.
The grounders will take in the lifted version of the node, and generate grounded copies of them.
The two grounders principally works the same:
1. Generate Parameter Permutations (i.e. all possible combination of objects for the node)
2. Foreach parameter permutation, make a copy of the original node and insert the new parameters into it.
The grounders need for there to either be objects declared in the problem, or constants in the domain (or both)
The [IParametized](../Models/PDDL/IParametized.cs) grounder also take into account if the permutation does not violate the actions static predicates.

#### Examples
Example of the predicate grounder:
```csharp
PDDLDecl decl = new PDDLDecl(...)
IGrounder<PredicateExp> grounder = new PredicateGrounder(decl);
PredicateExp predicate = new PredicateExp(...);
List<PredicateExp> groundedPredicates = grounder.Ground(predicate);
```

Example of the `IParametized` grounder with `ForAllExp` input:
```csharp
PDDLDecl decl = new PDDLDecl(...)
IGrounder<IParametized> grounder = new ParametizedGrounder(decl);
ForAllExp exp = new ForAllExp (...);
List<IParametized> groundedForAllExps = grounder.Ground(exp);
```
