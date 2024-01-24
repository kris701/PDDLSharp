Analysers are capable of checking if the syntax of a given structure is valid, or give suggestions to changes.

# PDDL Analyser
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.Analysers.PDDL-orange)

The PDDL Analyser takes in a [PDDLDecl](../Models/PDDL/PDDLDecl.cs) and analyses its structure.
It will give errors, warnings or messages depending on the severity of the issue.
As an example, it will tell you if types are missmatches or if a predicate is unused.
More generally, the analyser will look for:
* Correct type usage in predicates, actions, objects, etc.
* Check for unused predicates, objects, constants, etc.
* Check for undeclared predicates, objects, constants, etc.
* Check for unique names of predicates, objects, constants, etc.
* Messages for missing critical parts of the domain or problem (e.g. If no goal is declared or if there is no predicates)
You can find all the messages/warnings/errors in the [IErrorListener](../ErrorListeners/IErrorListener.cs) object.

#### Examples
To analyse a domain/problem (Note, the analyser will also contextualise the PDDL declaration, if it have not been contextualised):
```csharp
IErrorListener listener = new ErrorListener();
IParser<INode> parser = new PDDLParser(listener);
IAnalyser<PDDLDecl> analyser = new PDDLAnalyser(listener);
PDDLDecl decl = new PDDLDecl(
    parser.ParseAs<DomainDecl>(new FileInfo("domain.pddl")),
    parser.ParseAs<ProblemDecl>(new FileInfo("problem.pddl"))
)
analyser.Analyse(decl);
```

# SAS Analyser
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.Analysers.SAS-orange)

There is an SAS analyser, that can take in a [SASDecl](../Models/SAS/SASDecl.cs) (Usually from the [translator](../Translators/readme.md)) and give out some analysis information.

The SAS Analyser can:
* Check for valid general SAS structure (e.g. if there are any operators or not)
* Check if there are any actions that is applicable from the initial state
* Check if there are any actions that can result in the goal state
* Check if there is a relaxed plan for the [SASDecl](../Models/SAS/SASDecl.cs)

#### Examples
Example of taking in a domain/problem pddl file, translating it and then analysing it
```csharp
IErrorListener listener = new ErrorListener();
IParser<INode> parser = new PDDLParser(listener);
IAnalyser<SASDecl> analyser = new SASAnalyser(listener);
ITranslator<PDDLDecl, SASDecl> translator = new PDDLToSASTranslator();
PDDLDecl pddlDecl = new PDDLDecl(
    parser.ParseAs<DomainDecl>(new FileInfo("domain.pddl")),
    parser.ParseAs<ProblemDecl>(new FileInfo("problem.pddl"))
)
SASDecl sasDecl = translator.Translate(pddlDecl);
analyser.Analyse(sasDecl);
```