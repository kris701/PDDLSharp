using PDDL.Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDL.ErrorListeners;
using PDDL.Tools;
using PDDL.Models;
using PDDL.Models.Domain;
using PDDL.Models.Problem;
using PDDL.Models.AST;
using PDDL.ASTGenerators;
using PDDL.Contextualisers;

namespace PDDL.Parsers
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
            return new PDDLDecl(
                ParseDomain(domainFile),
                ParseProblem(problemFile));
        }

        public DomainDecl ParseDomain(string domainFile)
        {
            if (!PDDLFileHelper.IsFileDomain(domainFile))
                Listener.AddError(new ParseError(
                    $"Attempted file to parse was not a domain file!",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing));

            var absAST = ParseAsASTTree(domainFile, Listener);

            IVisitor<ASTNode, INode, IDecl> visitor = new DomainVisitor();
            var returnDomain = visitor.Visit(absAST, null, Listener);
            if (returnDomain is DomainDecl decl)
                return decl;
            return new DomainDecl(new ASTNode());
        }

        public ProblemDecl ParseProblem(string problemFile)
        {
            if (!PDDLFileHelper.IsFileProblem(problemFile))
                Listener.AddError(new ParseError(
                    $"Attempted file to parse was not a problem file!",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing));

            var absAST = ParseAsASTTree(problemFile, Listener);

            IVisitor<ASTNode, INode, IDecl> visitor = new ProblemVisitor();
            var returnProblem = visitor.Visit(absAST, null, Listener);
            if (returnProblem is ProblemDecl decl)
                return decl;
            return new ProblemDecl(new ASTNode());
        }

        private ASTNode ParseAsASTTree(string path, IErrorListener listener)
        {
            var text = ReadDataAsString(path, listener);

            IASTParser<ASTNode> astParser = new ASTParser();
            var absAST = astParser.Parse(text);
            return absAST;
        }

        private string ReadDataAsString(string path, IErrorListener listener)
        {
            if (!File.Exists(path))
            {
                listener.AddError(new ParseError(
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
            string returnStr = "";
            bool isComment = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ';')
                    isComment = true;
                else if (text[i] == ASTTokens.BreakToken)
                    isComment = false;

                if (isComment)
                    returnStr += ' ';
                else
                    returnStr += text[i];
            }
            return returnStr;
        }
    }
}
