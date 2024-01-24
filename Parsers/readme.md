PDDLSharp is equiped with a set of different parsers regarding planning. Each of the parsers work the same way, they can take in a file (or text) and parse it as specific node types.

The parsers also keep track of where in the source file a given construct is located at, by saving the start character, end character and line number of it.

# PDDL Parser
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.Parsers.PDDL-orange)

The PDDL Parser is the main part of PDDLSharp.
Its a fully fledged parser that can parser up to PDDL 2.2 files.
You can parse any of the [INode](../Models/PDDL/INode.cs) types with this parser.

#### Supported PDDL Requirements
Here is the set of requirements that the parser supports.
Do also note, these are also the requirements that the code generator, contextualiser and analyser supports (for PDDL).

- [x] STRIPS (`:strips`)
- [x] Typing (`:typing`)
- [x] Disjunctive Preconditions (`:disjunctive-preconditions`)
- [x] Equality (`:equality`)
- [x] Quantified Preconditions (`:quantified-preconditions`)
    - [x] Existential Preconditions (`:existential-preconditions`)
    - [x] Universal Preconditions (`:universal-preconditions`)
- [X] Conditional Effects (`:conditional-effects`)
- [X] Domain Axioms (`:domain-axioms`)
    - [ ] Subgoals Through Axioms (`:subgoals-through-axioms`)
    - [ ] Expression Evaluation (`:expression-evaluation`)
- [X] ADL (`:adl`)
- [X] Fluents (`:fluents`)
- [X] Durative Actions (`:durative-actions`)
    - [X] Durative Inequalities (`:durative-inequalities`)
    - [X] Continuous Effects (`:continuous-effects`)
- [X] Negative Preconditions (`:negative-preconditions`)
- [X] Derived Predicates (`:derived-predicates`)
- [X] Timed Initial Literals (`:timed-initial-literals`)
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
Example of how to parse a domain and problem file:
```csharp
IErrorListener listener = new ErrorListener();
IParser<INode> parser = new PDDLParser(listener);
PDDLDecl decl = new PDDLDecl(
    parser.ParseAs<DomainDecl>(new FileInfo("domain.pddl")),
    parser.ParseAs<ProblemDecl>(new FileInfo("problem.pddl"))
)
```

To parse a file as a specific PDDL object:
```csharp
IErrorListener listener = new ErrorListener();
IParser<INode> parser = new PDDLParser(listener);
ActionDecl decl = parser.ParseAs<ActionDecl>(new FileInfo("action-file.pddl"));
```

# Fast Downward Plan Parser
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.Parsers.FastDownward.Plan-orange)

There is also a plan parser that can parse [Fast Downward](https://www.fast-downward.org/) output plans.
This format is a series of grounded action sequences, as well as a total plan cost in the end.

An example of the plan format can be seen below:
```
(pick ball1 rooma left)
(pick ball2 rooma right)
(move rooma roomb)
(drop ball1 roomb left)
(drop ball2 roomb right)
; cost = 5 (unit cost)
```

#### Examples
Example of how to parse a plan file:
```csharp
IErrorListener listener = new ErrorListener();
IParser<ActionPlan> parser = new FastDownwardPlanParser(listener);
ActionPlan plan = parser.Parse(new FileInfo("planFile.plan"));
```

# Fast Downward SAS Parser
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.Parsers.FastDownward.SAS-orange)

There is also a SAS parser that can parse [Fast Downward](https://www.fast-downward.org/) intermediate output SAS format.
This format is a sort of fully grounded version of the lifted PDDL format.

An example of this format is the [INode](../Models/FastDownward/SAS/Sections/VersionDecl.cs) section in the translator format:
```SAS
...
begin_version
3
end_version
...
```

#### Examples
Example of how to parse a SAS file:
```csharp
IErrorListener listener = new ErrorListener();
IParser<ISASNode> parser = new SASParser(listener);
SASDecl sas = parser.ParseAs<SASDecl>(new FileInfo("file.sas"));
```