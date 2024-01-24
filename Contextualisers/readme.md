A contextualiser is able to take in one format, and enhance it in some way.

# PDDL Contextualiser
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.Contextualisers.PDDL-orange)

The PDDL Contextualiser is able to give additional context to domains and problems, such as decorating correct types across all subnodes based on parameters. In details, the PDL contextualiser does the following:
* Decorate all constants in a [ProblemDecl](../Models/PDDL/Problem/ProblemDecl.cs) with types.
* Inserts the `=` predicate (hidden, but needed for the analyser).
* Decorates all content of [ActionDecl](../Models/PDDL/Domain/ActionDecl.cs), [DurativeActionDecl](../Models/PDDL/Domain/DurativeActionDecl.cs) and [AxiomDecl](../Models/PDDL/Domain/AxiomDecl.cs) so that subnodes matches the declared parameter types of the action.
* Converts all derived predicates into a [DerivedPredicateExp](../Models/PDDL/Expressions/DerivedPredicateExp.cs).
* Decorates all predicates in the [ProblemDecl](../Models/PDDL/Problem/ProblemDecl.cs) with object types
* Decorates all subnodes of the [GoalDecl](../Models/PDDL/Problem/GoalDecl.cs) in the [ProblemDecl](../Models/PDDL/Problem/ProblemDecl.cs) with types.

#### Examples
To contextualise a domain/problem:
```csharp
IErrorListener listener = new ErrorListener();
IParser<INode> parser = new PDDLParser(listener);
PDDLDecl decl = new PDDLDecl(
    parser.ParseAs<DomainDecl>(new FileInfo("domain.pddl")),
    parser.ParseAs<ProblemDecl>(new FileInfo("problem.pddl"))
)
contextualiser.Contexturalise(decl);
```