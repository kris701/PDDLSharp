# Plan Validator
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.PDDLSharp.Toolkit.PlanValidators-orange)

The plan validator in PDDLSharps builds on the State Space Simulator described above.
However, instead of giving it individual actions to execute, it takes in a `ActionPlan` that can be parsed from the `FastDownwardPlanParser`.
It will then return true if the plan is valid, and false if the plan was invalid.
This validator does not throw errors or explain why a plan is invalid, you can use the State Space Simulator for that.

#### Examples
```csharp
IErrorListener listener = new ErrorListener();
IParser<ActionPlan> parser = new FastDownwardPlanParser(listener);
ActionPlan plan = parser.Parse(new FileInfo("planFile"));

PDDLDecl declaration = new PDDLDecl(...);
IPlanValidator validator = new PlanValidator();
bool isValid = validator.Validate(plan, declaration);
```