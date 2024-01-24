There is also a series of code generators, that can output the same formats as all the parser could parse in to begin with.
They all work the same way, where you simply give the required format (e.g. a `DomainDecl` object) and outputs it as a file or text.

# PDDL Code Generator
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.CodeGenerators.PDDL-orange)

The PDDL Code Generator can take in any instance of an [INode](../Models/PDDL/INode.cs) and generate appropriate PDDL code for it.
This does not have to be a [DomainDecl](../Models/PDDL/Domain/DomainDecl.cs), but it can be lower nodes, such as if you want to generate just code for a [ActionDecl](../Models/PDDL/Domain/ActionDecl.cs).

#### Examples

```csharp
IErrorListener listener = new ErrorListener();
ICodeGenerator<INode> generator = new PDDLCodeGenerator(listener);
PDDLDecl decl = new PDDLDecl(...);
// If you want a "pretty" output, use:
// generator.Readable = true;
generator.Generate(decl.Domain, "domain.pddl");
generator.Generate(decl.Problem, "problem.pddl");
```

```csharp
IErrorListener listener = new ErrorListener();
ICodeGenerator<INode> generator = new PDDLCodeGenerator(listener);
ActionDecl action = new ActionDecl (...);
generator.Generate(action , "action.pddl");
```

# Fast Downward Plan Code Generator
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.CodeGenerators.FastDownward.Plans-orange)

The code generator for Plans generate a [Fast Downward](https://www.fast-downward.org/) compatible plan. That consists of:
* A list of grounded actions
* A final action cost in the end.

As an example, this generator can make the format:
```
(pick ball1 rooma left)
(pick ball2 rooma right)
(move rooma roomb)
(drop ball1 roomb left)
(drop ball2 roomb right)
; cost = 5 (unit cost)
```

Do note, the `Readable` property does nothing for this generator.

#### Examples
```csharp
IErrorListener listener = new ErrorListener();
ICodeGenerator<ActionPlan> generator = new FastDownwardPlanGenerator(listener);
ActionPlan plan = new ActionPlan(...);
generator.Generate(plan, "planFile");
```

# Fast Downward SAS Code Generator
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.CodeGenerators.FastDownward.SAS-orange)

The SAS code generator generates a [Fast Downward](https://www.fast-downward.org/) compliant [translator](https://www.fast-downward.org/TranslatorOutputFormat) SAS code.
The general structure of this format is a bunch of `begin_` and `end_` blocks, that describes a grounded version of a domain+problem.

An example of this format is the [VersionDecl](../Models/FastDownward/SAS/Sections/VersionDecl.cs) section in the translator format:
```SAS
...
begin_version
3
end_version
...
```

Do note, the `Readable` property does nothing for this generator.

#### Examples
```csharp
IErrorListener listener = new ErrorListener();
ICodeGenerator<ISASNode> generator = new SASCodeGenerator(listener);
SASDecl sas = new SASDecl(...);
generator.Generate(sas, "output.sas");
```