using PDDLSharp.ASTGenerators;
using PDDLSharp.ASTGenerators.SAS;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.SAS;

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
