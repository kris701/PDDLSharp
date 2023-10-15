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

        public INode? VisitSections(ASTNode node)
        {
            INode? returnNode;
            if ((returnNode = TryParseAsVersion(node)) != null) return returnNode;
            if ((returnNode = TryParseAsMetric(node)) != null) return returnNode;
            if ((returnNode = TryParseAsVariable(node)) != null) return returnNode;
            if ((returnNode = TryParseAsMutex(node)) != null) return returnNode;
            if ((returnNode = TryParseAsInitState(node)) != null) return returnNode;
            if ((returnNode = TryParseAsGoalState(node)) != null) return returnNode;
            if ((returnNode = TryParseAsOperator(node)) != null) return returnNode;
            if ((returnNode = TryParseAsAxiom(node)) != null) return returnNode;

            Listener.AddError(new PDDLSharpError(
                $"Could not parse content of AST node: {node.OuterContent}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing));
            return returnNode;
        }

        private INode? TryParseAsVersion(ASTNode node)
        {
            if (IsNodeOfType(node, "version") &&
                MustBeDigitsOnly(node.InnerContent, "version"))
            {
                var version = int.Parse(node.InnerContent);
                return new VersionDecl(node, version);
            }
            return null;
        }

        private INode? TryParseAsMetric(ASTNode node)
        {
            if (IsNodeOfType(node, "metric") &&
                MustBeDigitsOnly(node.InnerContent, "metric"))
            {
                var choice = int.Parse(node.InnerContent);
                return new MetricDecl(node, choice == 1);
            }
            return null;
        }

        private INode? TryParseAsVariable(ASTNode node)
        {
            if (IsNodeOfType(node, "variable"))
            {
                var lineSplit = node.InnerContent.Split(SASASTTokens.BreakToken);
                if (lineSplit.Length < 3)
                    Listener.AddError(new PDDLSharpError(
                        $"Variable node must be at least 3 lines, but it has {lineSplit.Length} lines.",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing));
                else
                {
                    if (MustBeDigitsOnly(lineSplit[1], "variable axiom layer") &&
                        MustBeDigitsOnly(lineSplit[2], "variable range"))
                    {
                        var variableName = lineSplit[0];
                        var axiomLayer = int.Parse(lineSplit[1]);
                        var variableRange = int.Parse(lineSplit[2]);
                        List<string> symbolicNames = new List<string>();
                        foreach (var item in lineSplit.Skip(3))
                            symbolicNames.Add(item);

                        return new VariableDecl(node, variableName, axiomLayer, variableRange, symbolicNames);
                    }
                }
            }
            return null;
        }

        private INode? TryParseAsMutex(ASTNode node)
        {
            if (IsNodeOfType(node, "mutex_group"))
            {
                var lineSplit = node.InnerContent.Split(SASASTTokens.BreakToken);
                if (lineSplit.Length < 1)
                    Listener.AddError(new PDDLSharpError(
                        $"Mutex group node must be at least 1 line, but it has {lineSplit.Length} lines.",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing));
                else
                {
                    List<ValuePair> group = new List<ValuePair>();
                    foreach (var item in lineSplit.Skip(1))
                    {
                        var split = item.Trim().Split(' ');
                        group.Add(new ValuePair(int.Parse(split[0]), int.Parse(split[1])));
                    }

                    return new MutexDecl(node, group);
                }
            }
            return null;
        }

        private INode? TryParseAsInitState(ASTNode node)
        {
            if (IsNodeOfType(node, "state"))
            {
                var lineSplit = node.InnerContent.Split(SASASTTokens.BreakToken);
                List<int> state = new List<int>();
                foreach (var var in lineSplit)
                    state.Add(int.Parse(var));
                return new InitStateDecl(node, state);
            }
            return null;
        }

        private INode? TryParseAsGoalState(ASTNode node)
        {
            if (IsNodeOfType(node, "goal"))
            {
                var lineSplit = node.InnerContent.Split(SASASTTokens.BreakToken);
                if (lineSplit.Length < 1)
                    Listener.AddError(new PDDLSharpError(
                        $"Goal state node must be at least 1 line, but it has {lineSplit.Length} lines.",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing));
                else
                {
                    List<ValuePair> goals = new List<ValuePair>();
                    foreach (var item in lineSplit.Skip(1))
                    {
                        var split = item.Trim().Split(' ');
                        goals.Add(new ValuePair(int.Parse(split[0]), int.Parse(split[1])));
                    }

                    return new GoalStateDecl(node, goals);
                }
            }
            return null;
        }

        private INode? TryParseAsOperator(ASTNode node)
        {
            if (IsNodeOfType(node, "operator"))
            {
                var lineSplit = node.InnerContent.Split(SASASTTokens.BreakToken);
                if (lineSplit.Length < 4)
                    Listener.AddError(new PDDLSharpError(
                        $"Operator node must be at least 4 lines, but it has {lineSplit.Length} lines.",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing));
                else
                {
                    var name = lineSplit[0];
                    int offset = 0;

                    List<ValuePair> pervails = new List<ValuePair>();
                    if (lineSplit[1] != "0")
                    {
                        var count = int.Parse(lineSplit[1]);
                        for(int i = 2; i < count + 2; i++)
                        {
                            var split = lineSplit[i].Trim().Split(' ');
                            pervails.Add(new ValuePair(int.Parse(split[0]), int.Parse(split[1])));
                        }
                        offset += count;
                    }

                    List<OperatorEffect> effects = new List<OperatorEffect>();
                    if (lineSplit[2 + offset] != "0")
                    {
                        var count = int.Parse(lineSplit[2 + offset]);
                        for (int i = 2 + offset; i < count + 2 + offset; i++)
                        {
                            List<ValuePair> effectConditions = new List<ValuePair>();

                            var split = lineSplit[i].Trim().Split(' ');
                            int effectOffset = 0;
                            if (split[0] != "0")
                            {
                                var effectConditionCount = int.Parse(split[0]);
                                for(int j = 1; j < effectConditionCount; j += 2)
                                {
                                    effectConditions.Add(new ValuePair(int.Parse(split[i]), int.Parse(split[i + 1])));
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

                    int cost = int.Parse(lineSplit[3 + offset]);

                    return new OperatorDecl(node, name, pervails, effects, cost);
                }
            }
            return null;
        }

        private INode? TryParseAsAxiom(ASTNode node)
        {
            if (IsNodeOfType(node, "rule"))
            {
                var lineSplit = node.InnerContent.Split(SASASTTokens.BreakToken);
                if (lineSplit.Length < 2)
                    Listener.AddError(new PDDLSharpError(
                        $"Axiom node must be at least 2 lines, but it has {lineSplit.Length} lines.",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing));
                else
                {
                    List<ValuePair> conditions = new List<ValuePair>();
                    int offset = 0;
                    foreach (var item in lineSplit.Skip(1))
                    {
                        var split = item.Trim().Split(' ');
                        if (split.Length == 3)
                            break;
                        conditions.Add(new ValuePair(int.Parse(split[0]), int.Parse(split[1])));
                        offset++;
                    }
                    var lastSplit = lineSplit[1 + offset].Split(' ');
                    var effectedVariable = int.Parse(lastSplit[0]);
                    var variablePrecondition = int.Parse(lastSplit[1]);
                    var newVariableValue = int.Parse(lastSplit[2]);

                    return new AxiomDecl(node, conditions, effectedVariable, variablePrecondition, newVariableValue);
                }
            }
            return null;
        }

        private bool IsNodeOfType(ASTNode node, string name)
        {
            return node.OuterContent.StartsWith($"begin_{name}") &&
                   node.OuterContent.EndsWith($"end_{name}");
        }

        private bool MustBeDigitsOnly(string str, string nodeType)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
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
    }
}
