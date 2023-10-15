using PDDLSharp.ASTGenerators.SAS;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.SAS;
using PDDLSharp.Models.SAS.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Parsers.SAS
{
    public class SectionVisitor
    {
        public IErrorListener Listener { get; set; }

        public SectionVisitor(IErrorListener listener)
        {
            Listener = listener;
        }

        public ISASNode? VisitAs<T>(ASTNode node) where T : ISASNode =>
            // Domain
            typeof(T) == typeof(SASDecl)        ? TryVisitAsSASDecl(node) :
            typeof(T) == typeof(VersionDecl)    ? TryVisitAsVersion(node) :
            typeof(T) == typeof(MetricDecl)     ? TryVisitAsMetric(node) :
            typeof(T) == typeof(VariableDecl)   ? TryVisitAsVariable(node) :
            typeof(T) == typeof(MutexDecl)      ? TryVisitAsMutex(node) :
            typeof(T) == typeof(InitStateDecl)  ? TryVisitAsInitState(node) :
            typeof(T) == typeof(GoalStateDecl)  ? TryVisitAsGoalState(node) :
            typeof(T) == typeof(OperatorDecl)   ? TryVisitAsOperator(node) :
            typeof(T) == typeof(AxiomDecl)      ? TryVisitAsAxiom(node) :
            VisitSections(node);

        public ISASNode? VisitSections(ASTNode node)
        {
            ISASNode? returnNode;
            if ((returnNode = TryVisitAsSASDecl(node)) != null) return returnNode;
            if ((returnNode = TryVisitAsVersion(node)) != null) return returnNode;
            if ((returnNode = TryVisitAsMetric(node)) != null) return returnNode;
            if ((returnNode = TryVisitAsVariable(node)) != null) return returnNode;
            if ((returnNode = TryVisitAsMutex(node)) != null) return returnNode;
            if ((returnNode = TryVisitAsInitState(node)) != null) return returnNode;
            if ((returnNode = TryVisitAsGoalState(node)) != null) return returnNode;
            if ((returnNode = TryVisitAsOperator(node)) != null) return returnNode;
            if ((returnNode = TryVisitAsAxiom(node)) != null) return returnNode;

            Listener.AddError(new PDDLSharpError(
                $"Could not parse content of AST node: {node.OuterContent}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing));
            return returnNode;
        }

        public ISASNode? TryVisitAsSASDecl(ASTNode node)
        {
            if (node.Children.Count != 0)
            {
                var retNode = new SASDecl(node);
                foreach (var child in node.Children)
                {
                    var visited = VisitSections(child);

                    switch (visited)
                    {
                        case VersionDecl n: retNode.Version = n; break;
                        case MetricDecl n: retNode.Metric = n; break;
                        case VariableDecl n: retNode.Variables.Add(n); break;
                        case MutexDecl n: retNode.Mutexes.Add(n); break;
                        case InitStateDecl n: retNode.InitState = n; break;
                        case GoalStateDecl n: retNode.GoalState = n; break;
                        case OperatorDecl n: retNode.Operators.Add(n); break;
                        case AxiomDecl n: retNode.Axioms.Add(n); break;
                    }
                }
                return retNode;
            }
            return null;
        }

        public ISASNode? TryVisitAsVersion(ASTNode node)
        {
            if (IsNodeOfType(node, "version") &&
                MustBeDigitsOnly(node.InnerContent, "version") &&
                MustNotContainEmptyLines(node.InnerContent, "version"))
            {
                var version = int.Parse(node.InnerContent);
                return new VersionDecl(node, version);
            }
            return null;
        }

        public ISASNode? TryVisitAsMetric(ASTNode node)
        {
            if (IsNodeOfType(node, "metric") &&
                MustBeDigitsOnly(node.InnerContent, "metric") &&
                MustNotContainEmptyLines(node.InnerContent, "metric"))
            {
                var choice = int.Parse(node.InnerContent);
                if (choice != 0 && choice != 1)
                    Listener.AddError(new PDDLSharpError(
                        $"Metric value must be 1 or 0, but it was {choice}.",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing));
                else
                    return new MetricDecl(node, choice == 1);
            }
            return null;
        }

        public ISASNode? TryVisitAsVariable(ASTNode node)
        {
            if (IsNodeOfType(node, "variable") &&
                MustNotContainEmptyLines(node.InnerContent, "variable") &&
                MustContainAtLeastNLines(node.InnerContent, "variable", 3))
            {
                var lineSplit = node.InnerContent.Split(SASASTTokens.BreakToken);
                if (MustBeDigitsOnly(lineSplit[1], "variable axiom layer") &&
                    MustBeDigitsOnly(lineSplit[2], "variable range"))
                {
                    int variableRange = int.Parse(lineSplit[2]);
                    if (variableRange != lineSplit.Length - 3)
                    {
                        Listener.AddError(new PDDLSharpError(
                            $"Variable range did not match the declared! Got '{lineSplit.Length - 3}' but expected '{variableRange}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Parsing));
                    }
                    else
                    {
                        var variableName = lineSplit[0].Trim();
                        var axiomLayer = int.Parse(lineSplit[1]);
                        List<string> symbolicNames = new List<string>();
                        foreach (var item in lineSplit.Skip(3))
                            symbolicNames.Add(item.Trim());

                        return new VariableDecl(node, variableName, axiomLayer, symbolicNames);
                    }
                }
            }
            return null;
        }

        public ISASNode? TryVisitAsMutex(ASTNode node)
        {
            if (IsNodeOfType(node, "mutex_group") &&
                MustNotContainEmptyLines(node.InnerContent, "mutex_group") &&
                MustContainAtLeastNLines(node.InnerContent, "mutex_group", 1))
            {
                var lineSplit = node.InnerContent.Split(SASASTTokens.BreakToken);
                if (MustBeDigitsOnly(lineSplit[0], "mutex_group"))
                {
                    int count = int.Parse(lineSplit[0]);
                    if (count != lineSplit.Length - 1)
                    {
                        Listener.AddError(new PDDLSharpError(
                            $"Mutex fact count did not match the declared! Got '{lineSplit.Length - 1}' but expected '{count}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Parsing));
                    }
                    else
                    {
                        List<ValuePair> group = ParseLinesAsPairs(lineSplit.Skip(1).ToList());

                        return new MutexDecl(node, group);
                    }
                }
            }
            return null;
        }

        public ISASNode? TryVisitAsInitState(ASTNode node)
        {
            if (IsNodeOfType(node, "state"))
            {
                var lineSplit = node.InnerContent.Split(SASASTTokens.BreakToken);
                List<int> state = new List<int>();
                if (lineSplit[0].Trim() != "")
                    foreach (var var in lineSplit)
                        state.Add(int.Parse(var));
                return new InitStateDecl(node, state);
            }
            return null;
        }

        public ISASNode? TryVisitAsGoalState(ASTNode node)
        {
            if (IsNodeOfType(node, "goal") &&
                MustNotContainEmptyLines(node.InnerContent, "goal") &&
                MustContainAtLeastNLines(node.InnerContent, "goal", 1))
            {
                var lineSplit = node.InnerContent.Split(SASASTTokens.BreakToken);
                int count = int.Parse(lineSplit[0]);
                if (count != lineSplit.Length - 1)
                {
                    Listener.AddError(new PDDLSharpError(
                        $"Goal count did not match the declared! Got '{lineSplit.Length - 1}' but expected '{count}'",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing));
                }
                else
                {
                    List<ValuePair> goals = ParseLinesAsPairs(lineSplit.Skip(1).ToList());
                    return new GoalStateDecl(node, goals);
                }
            }
            return null;
        }

        public ISASNode? TryVisitAsOperator(ASTNode node)
        {
            if (IsNodeOfType(node, "operator") &&
                MustNotContainEmptyLines(node.InnerContent, "operator") &&
                MustContainAtLeastNLines(node.InnerContent, "operator", 4))
            {
                var lineSplit = node.InnerContent.Split(SASASTTokens.BreakToken);
                var name = lineSplit[0].Trim();
                int offset = 0;

                List<ValuePair> pervails = new List<ValuePair>();
                try
                {
                    var pervailsCount = int.Parse(lineSplit[1]);
                    if (pervailsCount != 0)
                    {
                        pervails.AddRange(ParseLinesAsPairs(lineSplit.ToList().GetRange(2, pervailsCount)));
                        offset += pervailsCount;
                    }
                }
                catch
                {
                    Listener.AddError(new PDDLSharpError(
                        $"Operator prevails was malformed! Either the prevail count did not match or some of the pairs are badly typed",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing));
                }

                List<OperatorEffect> effects = new List<OperatorEffect>();
                try
                {
                    if (lineSplit[2 + offset] != "0")
                    {
                        var count = int.Parse(lineSplit[2 + offset]);
                        for (int i = 2 + offset + 1; i <= count + 2 + offset; i++)
                        {
                            List<ValuePair> effectConditions = new List<ValuePair>();

                            var split = lineSplit[i].Trim().Split(' ');
                            int effectOffset = 0;
                            if (split[0] != "0")
                            {
                                var effectConditionCount = int.Parse(split[0]);
                                for (int j = 1; j < effectConditionCount; j += 2)
                                {
                                    effectConditions.Add(new ValuePair(int.Parse(split[j]), int.Parse(split[j + 1])));
                                }
                                effectOffset += effectConditionCount;
                            }

                            var effectedVariable = int.Parse(split[1 + effectOffset]);
                            var variablePrecondition = int.Parse(split[2 + effectOffset]);
                            var variableEffect = int.Parse(split[3 + effectOffset]);

                            effects.Add(new OperatorEffect(effectConditions, effectedVariable, variablePrecondition, variableEffect));
                        }
                        offset += count;
                    }
                }
                catch
                {
                    Listener.AddError(new PDDLSharpError(
                        $"Operator effects was malformed! Either the effect count did not match or some of the string is badly typed",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing));
                }

                if (MustBeDigitsOnly(lineSplit[3 + offset], "operator cost"))
                {
                    int cost = int.Parse(lineSplit[3 + offset]);
                    return new OperatorDecl(node, name, pervails, effects, cost);
                }
            }
            return null;
        }

        public ISASNode? TryVisitAsAxiom(ASTNode node)
        {
            if (IsNodeOfType(node, "rule") &&
                MustNotContainEmptyLines(node.InnerContent, "rule") &&
                MustContainAtLeastNLines(node.InnerContent, "rule", 2))
            {
                var lineSplit = node.InnerContent.Split(SASASTTokens.BreakToken);
                int count = int.Parse(lineSplit[0]);
                if (count != lineSplit.Length - 2)
                {
                    Listener.AddError(new PDDLSharpError(
                        $"Axiom fact count did not match the declared! Got '{lineSplit.Length - 2}' but expected '{count}'",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing));
                }
                else
                {
                    List<ValuePair> conditions = ParseLinesAsPairs(lineSplit.ToList().GetRange(1, count));
                    var lastSplit = lineSplit[1 + count].Split(' ').ToList();
                    lastSplit.RemoveAll(x => x.Trim() == "");
                    if (lastSplit.Count != 3)
                    {
                        Listener.AddError(new PDDLSharpError(
                            $"Axiom denotion did not match the declared! Got '{lastSplit.Count}' but expected '3'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Parsing));
                    }
                    else
                    {
                        var effectedVariable = int.Parse(lastSplit[0]);
                        var variablePrecondition = int.Parse(lastSplit[1]);
                        var newVariableValue = int.Parse(lastSplit[2]);

                        return new AxiomDecl(node, conditions, effectedVariable, variablePrecondition, newVariableValue);
                    }
                }
            }
            return null;
        }

        private List<ValuePair> ParseLinesAsPairs(List<string> lines)
        {
            var pairs = new List<ValuePair>();
            foreach (var item in lines)
            {
                var split = item.Trim().Split(' ');
                pairs.Add(new ValuePair(int.Parse(split[0]), int.Parse(split[1])));
            }
            return pairs;
        }

        private bool IsNodeOfType(ASTNode node, string name)
        {
            return node.OuterContent.Trim().StartsWith($"begin_{name}") &&
                   node.OuterContent.Trim().EndsWith($"end_{name}");
        }

        private bool MustBeDigitsOnly(string str, string nodeType)
        {
            foreach (char c in str.Trim())
            {
                if ((c < '0' || c > '9') && c != '-')
                {
                    Listener.AddError(new PDDLSharpError(
                        $"Node '{nodeType}' must be digits only, but got '{str}'.",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing));
                    return false;
                }
            }
            return true;
        }

        private bool MustNotContainEmptyLines(string str, string nodeType)
        {
            var lines = str.Split(SASASTTokens.BreakToken);
            if (lines.Any(x => x.Replace($"{SASASTTokens.BreakToken}", "").Trim() == ""))
            {
                Listener.AddError(new PDDLSharpError(
                    $"No lines must be empty! But node '{nodeType}' contains empty lines.",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing));
                return false;
            }
            return true;
        }

        private bool MustContainAtLeastNLines(string str, string nodeType, int n)
        {
            var lines = str.Split(SASASTTokens.BreakToken);
            if (lines.Length < n)
            {
                Listener.AddError(new PDDLSharpError(
                    $"Node '{nodeType}' must contain at least {n} lines but got {lines}!",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing));
                return false;
            }
            return true;
        }
    }
}
