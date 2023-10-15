using PDDLSharp.ASTGenerators;
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
    public class SASParser : BaseParser<SASDecl>
    {
        public SASParser(IErrorListener listener) : base(listener)
        {
        }

        public override SASDecl Parse(string file)
        {
            IGenerator astParser = new SASASTGenerator(Listener);
            var absAST = astParser.Generate(new FileInfo(file));
            var retNode = new SASDecl(absAST);

            var visitor = new SectionVisitor(Listener);

            foreach (var child in absAST.Children)
            {
                var visited = visitor.VisitSections(child);

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
    }
}
