﻿using PDDLSharp.ASTGenerators;
using PDDLSharp.ASTGenerators.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers.Visitors;
using PDDLSharp.Tools;

namespace PDDLSharp.Parsers.PDDL
{
    public class PDDLParser : BaseParser<INode>
    {
        public PDDLParser(IErrorListener listener) : base(listener)
        {
        }

        public PDDLDecl ParseDecl(FileInfo domainFile, FileInfo problemFile)
        {
            if (!PDDLFileHelper.IsFileDomain(domainFile.FullName))
                Listener.AddError(new PDDLSharpError(
                    $"File is not a domain file: '{domainFile}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing));
            if (!PDDLFileHelper.IsFileProblem(problemFile.FullName))
                Listener.AddError(new PDDLSharpError(
                    $"File is not a problem file: '{problemFile}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing));

            if (!CompatabilityHelper.IsPDDLDomainSpported(File.ReadAllText(domainFile.FullName)))
                Listener.AddError(new PDDLSharpError(
                    $"Domain contains unsupported packages! Results may not be accurate!",
                    ParseErrorType.Warning,
                    ParseErrorLevel.PreParsing));

            return new PDDLDecl(
                ParseAs<DomainDecl>(domainFile),
                ParseAs<ProblemDecl>(problemFile));
        }

        public override U ParseAs<U>(string text)
        {
            IGenerator astParser = new PDDLASTGenerator(Listener);
            astParser.SaveLinePlacements = SaveLinePlacements;
            var absAST = astParser.Generate(text);

            var visitor = new ParserVisitor(Listener);
            var result = visitor.TryVisitAs<U>(absAST, null);
            if (result is U act)
                return act;
            return default;
        }
    }
}
