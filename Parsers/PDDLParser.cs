using PDDLSharp.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Tools;
using PDDLSharp.Models;
using PDDLSharp.Models.AST;
using PDDLSharp.ASTGenerators;
using PDDLSharp.Contextualisers;
using PDDLSharp.Models.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.PDDL.Domain;

namespace PDDLSharp.Parsers
{
    public class PDDLParser : BaseParser<INode>
    {
        public PDDLParser(IErrorListener listener) : base(listener)
        {
        }

        public PDDLDecl ParseDecl(string domainFile, string problemFile)
        {
            if (!PDDLFileHelper.IsFileDomain(domainFile))
                Listener.AddError(new ParseError(
                    $"File is not a domain file: '{domainFile}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing));
            if (!PDDLFileHelper.IsFileProblem(problemFile))
                Listener.AddError(new ParseError(
                    $"File is not a problem file: '{problemFile}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing));

            if (!CompatabilityHelper.IsPDDLDomainSpported(File.ReadAllText(domainFile)))
                Listener.AddError(new ParseError(
                    $"Domain contains unsupported packages! Results may not be accurate!",
                    ParseErrorType.Warning,
                    ParseErrorLevel.PreParsing));

            return new PDDLDecl(
                ParseAs<DomainDecl>(domainFile),
                ParseAs<ProblemDecl>(problemFile));
        }

        public override INode Parse(string file)
        {
            return ParseAs<INode>(file);
        }

        public override U ParseAs<U>(string file)
        {
            IGenerator astParser = new ASTGenerator(Listener);
            var absAST = astParser.Generate(new FileInfo(file));

            var visitor = new ParserVisitor(Listener);
            var result = visitor.TryVisitAs<U>(absAST, null);
            if (result is U act)
                return act;
            return default(U);
        }
    }
}
