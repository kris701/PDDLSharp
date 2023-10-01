
[![Build and Publish](https://github.com/kris701/PDDLSharp/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/kris701/PDDLSharp/actions/workflows/dotnet-desktop.yml)
![Nuget](https://img.shields.io/nuget/v/PDDLSharp)
![Nuget](https://img.shields.io/nuget/dt/PDDLSharp)

# PDDLSharp

This is a project to make a PDDL parser, contextualiser, analyser and code generator for C#. 
The parser is fully PDDL 2.2 compatible.

## Examples
A usage example of how to use the parser:
```csharp
IErrorListener listener = new ErrorListener();
IParser parser = new PDDLParser(listener);
PDDLDecl decl = parser.Parse("domain-file.pddl", "problem-file.pddl");
```

To parse a file as a specific PDDL object:
```csharp
IErrorListener listener = new ErrorListener();
IParser parser = new PDDLParser(listener);
ActionDecl decl = parser.ParseAs<ActionDecl>("action-file.pddl");
```

To contextualise a domain/problem:
```csharp
IErrorListener listener = new ErrorListener();
IParser parser = new PDDLParser(listener);
PDDLDecl decl = parser.Parse("domain-file.pddl", "problem-file.pddl");
contextualiser.Contexturalise(decl);
```

To analyse a domain/problem:
```csharp
IErrorListener listener = new ErrorListener();
IParser parser = new PDDLParser(listener);
IAnalyser analyser = new PDDLAnalyser(listener);
PDDLDecl decl = parser.Parse("domain-file.pddl", "problem-file.pddl");
contextualiser.Contexturalise(decl);
analyser.Analyse(decl);
```

To generate PDDL code from a PDDL object:
```csharp
IErrorListener listener = new ErrorListener();
ICodeGenerator generator = new PDDLCodeGenerator(listener);
PDDLDecl decl = new PDDLDecl(...);
// If you want a "pretty" output, use:
// generator.Readable = true;
generator.Generate(decl.Domain, "domain.pddl");
generator.Generate(decl.Problem, "problem.pddl");
```

## Supported Requirements
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