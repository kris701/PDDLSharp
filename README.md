
<p align="center">
    <img src="https://github.com/kris701/PDDLSharp/assets/22596587/6c7c3516-bb1e-4713-ad17-e2eaff67107b" width="200" height="200" />
</p>

[![Build and Publish](https://github.com/kris701/PDDLSharp/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/kris701/PDDLSharp/actions/workflows/dotnet-desktop.yml)
![Nuget](https://img.shields.io/nuget/v/PDDLSharp)
![Nuget](https://img.shields.io/nuget/dt/PDDLSharp)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/kris701/PDDLSharp/main)
![GitHub commit activity (branch)](https://img.shields.io/github/commit-activity/m/kris701/PDDLSharp)
![Static Badge](https://img.shields.io/badge/Platform-Windows-blue)
![Static Badge](https://img.shields.io/badge/Platform-Linux-blue)
![Static Badge](https://img.shields.io/badge/Framework-dotnet--7.0-green)

Welcome to PDDLSharp!
This wiki serves to document how to use different parts of PDDLSharp.
If you encounter any problems, please make sure you have read through the wiki first. If you can still not find an answer, feel free to make an [issue](https://github.com/kris701/PDDLSharp/issues).

# What is PDDL?
PDDL is a standardised format that is extensively used in planning. The general idea of it is to have a set of initial facts (predicates), a set of goal facts we want to each and a set of actions that modify facts. The PDDL format is split into two parts; a domain and a problem. The domain contains the definitions of the actions, as well as what predicates can be used. An example of a domain can be the well known domain Gripper. Gripper is a domain where the goal is to move balls to other rooms by means of a robot hand. It usually consists of 3 actions, `pick`, `move` and `drop`. An example of a [gripper domain](https://github.com/aibasel/downward-benchmarks/blob/master/gripper/domain.pddl) could be:
```PDDL
(define (domain gripper-strips)
   (:predicates (room ?r) (ball ?b) (gripper ?g)
		(at-robby ?r) (at ?b ?r) (free ?g) (carry ?o ?g)
   )

   (:action move
       :parameters (?from ?to)
       :precondition 
         (and 
            (room ?from) (room ?to) 
            (at-robby ?from)
         )
       :effect
         (and
            (at-robby ?to)
            (not (at-robby ?from))
         )
   )

   (:action pick
       :parameters (?obj ?room ?gripper)
       :precondition  
         (and 
            (ball ?obj) 
            (room ?room) 
            (gripper ?gripper)
            (at ?obj ?room) 
            (at-robby ?room) 
            (free ?gripper)
         )
       :effect 
         (and 
            (carry ?obj ?gripper)
            (not (at ?obj ?room)) 
            (not (free ?gripper))
         )
   )

   (:action drop
       :parameters (?obj ?room ?gripper)
       :precondition
         (and
            (ball ?obj) 
            (room ?room) 
            (gripper ?gripper)
            (carry ?obj ?gripper) 
            (at-robby ?room)
         )
       :effect 
         (and
            (at ?obj ?room)
            (free ?gripper)
            (not (carry ?obj ?gripper))
         )
   )
)
```
Where the predicate and actions definitions can be seen.

The other part is the problem declaration. This contains the initial facts and goal facts, so its more or less an instanciated version of the domain. An example of a [gripper problem](https://github.com/aibasel/downward-benchmarks/blob/master/gripper/prob01.pddl) can be seen below:
```PDDL
(define (problem strips-gripper-x-1)
   (:domain gripper-strips)
   (:objects rooma roomb ball4 ball3 ball2 ball1 left right)
   (:init (room rooma)
          (room roomb)
          (ball ball4)
          (ball ball3)
          (ball ball2)
          (ball ball1)
          (at-robby rooma)
          (free left)
          (free right)
          (at ball4 rooma)
          (at ball3 rooma)
          (at ball2 rooma)
          (at ball1 rooma)
          (gripper left)
          (gripper right))
   (:goal (and (at ball4 roomb)
               (at ball3 roomb)
               (at ball2 roomb)
               (at ball1 roomb))))
```
This example is just a very simple domain, PDDL contains many more constructs to make more advanced domains and problems or to extend their possibilities.

# Why PDDLSharp?
PDDLSharp is much more than just a parser for PDDL, it contains contextualisers, analysers, code generators, macro generators, plan validators, grounders and much more. And it is fully managed, open source C# code, so no mucking about with ANTLR grammars and weird return node formats.
PDDLSharp can serve as a solid and tested backbone for your own project, whether you just want to parse PDDL or use some other more advanced PDDLSharp components.

# How to install PDDLSharp?
There are a few ways to install PDDLSharp on.
You can either find it on [Nuget Package Manager](https://www.nuget.org/packages/PDDLSharp/), [GitHub Package Manager](https://github.com/kris701/PDDLSharp/pkgs/nuget/PDDLSharp) or simply download the source code and compile it yourself.

# How to use PDDLSharp?
All of PDDLSharp is a large class library that you can use whatever section of you need.
All of the project can be accessed through the common namespace `PDDLSharp.`.
Generally, most of the core components of PDDLSharp is based on an error listener, that can record several errors (such as warnings from the analyser) at the same time. It can be set to throw at a given level of error (e.g. throw if a warning or higher is given).
That said, you will find the general format of the components being as follows
```csharp
IErrorListener listener = new ErrorListener();
...
```
Since most of the components need a listener object given to them.

An example of how to parse a domain file with PDDLSharp:
```csharp
IErrorListener listener = new ErrorListener();
IParser<INode> parser = new PDDLParser(listener);
DomainDecl = parser.ParseAs<DomainDecl>(new FileInfo("domain.pddl"));
```
