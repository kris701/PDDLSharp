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
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Problem;
using PDDLSharp.Models.AST;
using PDDLSharp.ASTGenerators;
using PDDLSharp.Contextualisers;

namespace PDDLSharp.Parsers
{
    public class PDDLParser : IPDDLParser
    {
        public IErrorListener Listener { get; }

        public PDDLParser(IErrorListener listener)
        {
            Listener = listener;
        }

        public PDDLDecl Parse(string domainFile, string problemFile)
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

        public T ParseAs<T>(string file) where T : INode
        {
            var absAST = ParseAsASTTree(file);
            var visitor = new ParserVisitor(Listener);
            var result = visitor.TryVisitAs<T>(absAST, null);
            if (result is T act)
                return act;
            return default(T);
        }

        private ASTNode ParseAsASTTree(string path)
        {
            var text = ReadDataAsString(path);

            IGenerator<ASTNode> astParser = new ASTGenerator();
            var absAST = astParser.Parse(text);
            return absAST;
        }

        private string ReadDataAsString(string path)
        {
            if (!File.Exists(path))
            {
                Listener.AddError(new ParseError(
                    $"Could not find the file to parse: '{path}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing));
            }
            var text = File.ReadAllText(path);
            text = ReplaceSpecialCharacters(text);
            text = ReplaceCommentsWithWhiteSpace(text);
            text = text.ToLower();
            return text;
        }

        private string ReplaceSpecialCharacters(string text)
        {
            text = text.Replace('\r', ' ');
            text = text.Replace('\t', ' ');
            return text;
        }

        private string ReplaceCommentsWithWhiteSpace(string text)
        {
            var retStr = text;
            int offset = 0;
            while (retStr.Contains(";"))
            {
                int from = retStr.IndexOf(";", offset);
                int to = retStr.IndexOf(ASTTokens.BreakToken, from);
                retStr = retStr.Remove(from, to - from).Insert(from, new string(' ', to - from));
                offset = to + 1;
            }
            return retStr;
        }
    }
}
