using PDDLSharp.ASTGenerators;
using PDDLSharp.ASTGenerators.FastDownward.SAS;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.FastDownward.SAS;

namespace PDDLSharp.Parsers.FastDownward.SAS
{
    public class FDSASParser : BaseParser<ISASNode>
    {
        public FDSASParser(IErrorListener listener) : base(listener)
        {
        }

        public override U ParseAs<U>(string text)
        {
            IGenerator astParser = new SASASTGenerator(Listener);
            astParser.SaveLinePlacements = SaveLinePlacements;
            var absAST = astParser.Generate(text);

            var visitor = new SectionVisitor(Listener);
            var result = visitor.VisitAs<U>(absAST);
            if (result is U act)
                return act;
            return default;
        }
    }
}
