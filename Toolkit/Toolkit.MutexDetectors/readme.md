# Mutex Detector
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.PDDLSharp.Toolkit.MutexDetector-orange)

There is a simple predicate mutex detector included in PDDLSharp.
It is capable of finding "balanced" predicates in a [PDDLDecl](../../Models/PDDL/PDDLDecl.cs), and assumes they are mutexes.
The general process is:
1. Let C be a new list of candidate mutexes
2. Foreach action
   - Get all predicates in the action effects
   - Count how many `add` and `del` there is for the predicate name
   - If the amount of `add` and `del` is in balance, 
      - Then add the predice to C
      - Else, if C contains this predicate, remove it from C and blacklist it from reentering
3. Return C

As an example, for the `gripper` domain, the predicate `at-robby` is "effect balanced". Since that:
* The action `move`'s effects adds 1 and removes 1 `at-robby`
* The action `pick`'s effects does not touch the predicate
* The action `drop`'s effects does not touch the predicate

Hence, there was just as many `add` as `del` of the predicate `at-robby`, so its assumed to be a mutex.

#### Examples
An example of how to find mutexes in a domain:
```csharp
IErrorListener listener = new ErrorListener();
IParser<INode> parser = new PDDLParser(listener);
PDDLDecl decl = new PDDLDecl(...)

IMutexDetectors detector = new EffectBalanceMutexes();
List<PredicateExp> mutexes = detector.FindMutexes(decl);
```