# Classical Planners
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.PDDLSharp.Toolkit.Planners-orange)

PDDLSharp also includes a small set of simple planners. To get a SAS representation of a [PDDLDecl](../../Models/PDDL/PDDLDecl.cs), please use the [translator](../../Translators/PDDLToSASTranslator.cs) designed for it.
These planners is able to find solutions for SMALL problems.
The planners are:
* [GreedyBFS](./Search/Classical/GreedyBFS.cs): Greedy Best First Search
* [GreedyBFSUAR](./Search/Classical/GreedyBFSUAR.cs): Greedy Best First Search with [Under-Approximation Refinement (UAR)](https://ojs.aaai.org/index.php/ICAPS/article/view/13678)
* [GreedyBFSPO](./Search/Classical/GreedyBFSPO.cs): Greedy Best First Search with [Preferred Operators (PO)](https://ai.dmi.unibas.ch/papers/helmert-jair06.pdf)
* [GreedyBFSDHE](./Search/Classical/GreedyBFSDHE.cs): Greedy Best First Search with [Deferred Heuristic Evaluation (DHE)](https://ai.dmi.unibas.ch/papers/helmert-jair06.pdf)

For these planners, there is a set of heuristics as well.
* [hConstant](./Heuristics/hConstant.cs): Returns a given constant all the time
* [hDepth](./Heuristics/hDepth.cs): Simply returns a cost that is 1 lower than its parent
* [hFF](./Heuristics/hFF.cs): Returns a cost based on a solution to the [relaxed planning graph](https://www.youtube.com/watch?app=desktop&v=7XH60fuMlIM) for the problem
* [hAdd](./Heuristics/hAdd.cs): Retuns the sum of actions needed to achive every goal fact
* [hMax](./Heuristics/hMax.cs): Returns the highest amount of actions needed to achive a goal fact.
* [hGoal](./Heuristics/hGoal.cs): Returns the amount of goals that are achived in the given state, i.e. `h = allGoals - achivedGoals`
* [hPath](./Heuristics/hPath.cs): Returns the cost of the current branch being evaluated
* [hWeighted](./Heuristics/hWeighted.cs): Takes one of the previously given heuristics, and weights its result from a constant.

There are also a set of "collection heuristics" that runs on a set of heuristics:
* [hColMax](./HeuristicsCollections/hColMax.cs): Gets the highest heuristic value from a set of heuristics
* [hColSom](./HeuristicsCollections/hColSum.cs): Gets the sum of the heuristic values from a set of heuristics

Again do note, that these planners are very inefficient, and will run out of memory with larger problems.

#### Examples
To find a plan using the Greedy Best First Search engine:
```csharp
PDDLDecl decl = new PDDLDecl(...);
ITranslator translator = new PDDLToSASTranslator(true);
SASDecl sas = translator.Translate(decl);
using (var greedyBFS = new GreedyBFS(sas, new hFF(decl)))
{
   var plan = greedyBFS.Solve();
}
```

## Black Box Planners
PDDLSharp also includes black box planners. This is a type of planners where the heuristic gets seriously limited, by not allowing it to know any structural knowledge of the domain.

The current planners are:
* [GreedyBFS](./Search/BlackBox/GreedyBFS.cs): Greedy Best First Search
* [GreedyBFSFocused](./Search/BlackBox/GreedyBFSFocused.cs): Greedy Best First Search with [Focused Macros](https://arxiv.org/abs/2004.13242)

This also only support the [hGoal](./Heuristics/hGoal.cs) heuristic