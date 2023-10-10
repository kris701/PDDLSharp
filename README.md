
[![Build and Publish](https://github.com/kris701/PDDLSharp/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/kris701/PDDLSharp/actions/workflows/dotnet-desktop.yml)
![Nuget](https://img.shields.io/nuget/v/PDDLSharp)
![Nuget](https://img.shields.io/nuget/dt/PDDLSharp)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/kris701/PDDLSharp/main)
![GitHub commit activity (branch)](https://img.shields.io/github/commit-activity/m/kris701/PDDLSharp)

# PDDLSharp

This is a package to make a PDDL parser, contextualiser, analyser, code generator and much more for C#. 
The parser is fully PDDL 2.2 compatible. The package can be found through [Nuget](https://www.nuget.org/packages/PDDLSharp/) or the [Git](https://github.com/kris701/PDDLSharp/pkgs/nuget/PDDLSharp) package manager.

# Parsers

There is a few parsers included in PDDLSharp, each of them will be explained in the subsections.

## PDDL Parser
The PDDL Parser is the main part of PDDLSharp.
Its a fully fledged parser that can parser up to PDDL 2.2 files.

A usage example of how to use the PDDL parser:
```csharp
IErrorListener listener = new ErrorListener();
IParser<INode> parser = new PDDLParser(listener);
PDDLDecl decl = new PDDLDecl(
    parser.ParseAs<DomainDecl>("domain.pddl"),
    parser.ParseAs<ProblemDecl>("problem.pddl")
)
```

To parse a file as a specific PDDL object:
```csharp
IErrorListener listener = new ErrorListener();
IParser<INode> parser = new PDDLParser(listener);
ActionDecl decl = parser.ParseAs<ActionDecl>("action-file.pddl");
```

To contextualise a domain/problem:
```csharp
IErrorListener listener = new ErrorListener();
IParser<INode> parser = new PDDLParser(listener);
PDDLDecl decl = new PDDLDecl(
    parser.ParseAs<DomainDecl>("domain.pddl"),
    parser.ParseAs<ProblemDecl>("problem.pddl")
)
contextualiser.Contexturalise(decl);
```

To analyse a domain/problem (Note, the analyser will also contextualise the PDDL declaration, if it have not been contextualised):
```csharp
IErrorListener listener = new ErrorListener();
IParser<INode> parser = new PDDLParser(listener);
IAnalyser analyser = new PDDLAnalyser(listener);
PDDLDecl decl = new PDDLDecl(
    parser.ParseAs<DomainDecl>("domain.pddl"),
    parser.ParseAs<ProblemDecl>("problem.pddl")
)
analyser.Analyse(decl);
```

## Plan Parser
There is also a plan parser that can parse [Fast Downward](https://www.fast-downward.org/) output plans.
```csharp
IErrorListener listener = new ErrorListener();
IParser<ActionPlan> parser = new FastDownwardPlanParser(listener);
ActionPlan plan = parser.Parse("planFile");
```

# Code Generators

## PDDL Code Generator
To generate PDDL code from a PDDL declaration:
```csharp
IErrorListener listener = new ErrorListener();
ICodeGenerator<INode> generator = new PDDLCodeGenerator(listener);
PDDLDecl decl = new PDDLDecl(...);
// If you want a "pretty" output, use:
// generator.Readable = true;
generator.Generate(decl.Domain, "domain.pddl");
generator.Generate(decl.Problem, "problem.pddl");
```

## Plan Code Generator
To generate a [Fast Downward](https://www.fast-downward.org/) plan from a `ActionPlan` declaration:
```csharp
IErrorListener listener = new ErrorListener();
ICodeGenerator<ActionPlan> generator = new FastDownwardPlanGenerator(listener);
ActionPlan plan = new ActionPlan(...);
generator.Generate(plan, "planFile");
```

# Simulators

## State Space Simulator
There is a State Space Simulator included with PDDLSharp.
This is a simulator that is capable of simulating the state changes for each action execution.
If there are invalid arguments or type issues, the simulator will throw an exception.
```csharp
PDDLDecl declaration = new PDDLDecl(...);
IStateSpaceSimulator simulator = new StateSpaceSimulator(declaration);
simulator.Step("actionName", "obj1", "obj2");
```

You can also give it the output of the Plan Parser to step through:
```csharp
IErrorListener listener = new ErrorListener();
IParser<ActionPlan> parser = new FastDownwardPlanParser(listener);
ActionPlan plan = parser.Parse("planFile");

PDDLDecl declaration = new PDDLDecl(...);
IStateSpaceSimulator simulator = new StateSpaceSimulator(declaration);
simulator.ExecutePlan(plan);
```

## Plan Validator
There is a simple plan validator included in PDDLSharp.
It is capable of taking in a `ActionPlan` and a `PDDLDecl` and verify if the given plan is even possible or not.
```csharp
IErrorListener listener = new ErrorListener();
IParser<ActionPlan> parser = new FastDownwardPlanParser(listener);
ActionPlan plan = parser.Parse("planFile");

PDDLDecl declaration = new PDDLDecl(...);
IPlanValidator validator = new PlanValidator();
validator.Verify(plan, declaration);
```
The validator will throw an exception if it cannot verify a given plan.

# Supported Requirements
PDDLSharp supports a large set of requirements, all the way up to PDDL 2.2:

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