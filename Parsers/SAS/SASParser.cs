using PDDLSharp.ASTGenerators;
using PDDLSharp.ASTGenerators.SAS;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.SAS;
using PDDLSharp.Models.SAS.Sections;
using PDDLSharp.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Parsers.SAS
{
    public class SASParser : BaseParser<ISASNode>
    {
        public SASParser(IErrorListener listener) : base(listener)
        {
        }

        public override U ParseAs<U>(string text)
        {
            IGenerator astParser = new SASASTGenerator(Listener);
            var absAST = astParser.Generate(text);

            var visitor = new SectionVisitor(Listener);
            var result = visitor.VisitAs<U>(absAST);
            if (result is U act)
                return act;
            return default;
        }
    }
}
