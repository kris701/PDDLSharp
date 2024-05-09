# Toolkits
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.Toolkits-orange)

This project contains misc tools that can be used for planning things.

## Static Predicate Detector

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
