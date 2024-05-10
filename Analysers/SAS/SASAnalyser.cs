﻿using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.SAS;

namespace PDDLSharp.Analysers.SAS
{
    public class SASAnalyser : IAnalyser<SASDecl>
    {
        public IErrorListener Listener { get; internal set; }

        public SASAnalyser(IErrorListener listener)
        {
            Listener = listener;
        }

        public void Analyse(SASDecl decl)
        {
            CheckForBasicSAS(decl);

            // Reachability Tests
            InitReachabilityCheck(decl);
            GoalReachabilityCheck(decl);
        }

        public void CheckForBasicSAS(SASDecl decl)
        {
            if (decl.DomainVariables.Count == 0)
                Listener.AddError(new PDDLSharpError(
                    $"SAS contains no domain variables.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser));
            if (decl.Operators.Count == 0)
                Listener.AddError(new PDDLSharpError(
                    $"SAS contains no operators.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser));
            if (decl.Init.Count == 0)
                Listener.AddError(new PDDLSharpError(
                    $"SAS contains no init facts.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser));
            if (decl.Goal.Count == 0)
                Listener.AddError(new PDDLSharpError(
                    $"SAS contains no goal facts.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser));
        }

        public void InitReachabilityCheck(SASDecl decl)
        {
            foreach (var op in decl.Operators)
            {
                bool valid = true;
                foreach (var fact in op.Pre)
                {
                    if (!decl.Init.Contains(fact))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                    return;
            }
            Listener.AddError(new PDDLSharpError(
                $"No operator is applicable from the initial state!",
                ParseErrorType.Warning,
                ParseErrorLevel.Analyser));
        }

        public void GoalReachabilityCheck(SASDecl decl)
        {
            foreach (var goal in decl.Goal)
            {
                bool isValid = false;
                foreach (var op in decl.Operators)
                {
                    if (op.Add.Contains(goal))
                    {
                        isValid = true;
                        break;
                    }
                }
                if (!isValid)
                    Listener.AddError(new PDDLSharpError(
                        $"No operator can make goal fact '{goal}' true!",
                        ParseErrorType.Warning,
                        ParseErrorLevel.Analyser));
            }
        }
    }
}
