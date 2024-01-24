Translators are capable of taking in one format, and transform it into another.

# PDDL to SAS Translator
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.Translators-orange)

This is a translator that can convert a [PDDLDecl](../Models/PDDL/PDDLDecl.cs) into a [SASDecl](../Models/SAS/SASDecl.cs) format.

This is used for the [planners](https://github.com/kris701/PDDLSharp/wiki/8.-Toolkit#planners), since they work a lot better with the more "raw" SAS format.

There are also a few subcomponents in the translator project, that can be used for grounding or static predicate detection.

#### Supported PDDL Requirements
Here is the set of requirements that the translator supports.

- [x] STRIPS (`:strips`)
- [x] Typing (`:typing`)
- [X] Disjunctive Preconditions (`:disjunctive-preconditions`)
- [X] Equality (`:equality`)
- [x] Quantified Preconditions (`:quantified-preconditions`)
    - [x] Existential Preconditions (`:existential-preconditions`)
    - [x] Universal Preconditions (`:universal-preconditions`)
- [X] Conditional Effects (`:conditional-effects`)
- [ ] Domain Axioms (`:domain-axioms`)
    - [ ] Subgoals Through Axioms (`:subgoals-through-axioms`)
    - [ ] Expression Evaluation (`:expression-evaluation`)
- [X] ADL (`:adl`)
- [ ] Fluents (`:fluents`)
- [ ] Durative Actions (`:durative-actions`)
    - [ ] Durative Inequalities (`:durative-inequalities`)
    - [ ] Continuous Effects (`:continuous-effects`)
- [X] Negative Preconditions (`:negative-preconditions`)
- [ ] Derived Predicates (`:derived-predicates`)
- [ ] Timed Initial Literals (`:timed-initial-literals`)
- [ ] Action Expansions (`:action-expansions`)
- [ ] Foreach Expansions (`:forach-expansions`)
- [ ] DAG Expansions (`:dag-expansions`)
- [ ] Safety Constraints (`:safety-constraints`)
- [ ] Open World (`:open-world`)
- [ ] True Negation (`:true-negation`)
- [ ] UCPOP (`:ucpop`)
- [ ] Constraints (`:constraints`)
- [ ] Preferences (`:preferences`)

#### Examples
```csharp
IErrorListener listener = new ErrorListener();
IParser<INode> parser = new PDDLParser(listener);
PDDLDecl decl = new PDDLDecl(...)
ITranslator translator = new PDDLToSASTranslator();
SASDecl sas = translator.Translate(decl);
```

## Static Predicate Detector
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.PDDLSharp.Translators.Tools-orange)

There is a simple static predicate detector included in PDDLSharp.
It is able to find predicates that are not ever changed by action effects, durative action effects, and axiom implies.

#### Examples
Example of how to find a list of static predicates in a [PDDLDecl](../Models/PDDL/PDDLDecl.cs):
```csharp
IErrorListener listener = new ErrorListener();
IParser<INode> parser = new PDDLParser(listener);
PDDLDecl decl = new PDDLDecl(...)

List<PredicateExp> staticPredicates = SimpleStaticPredicateDetector.FindStaticPredicates(decl);
```

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
