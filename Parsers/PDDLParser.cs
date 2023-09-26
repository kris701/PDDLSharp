using Parsers.Visitors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLParser.Analysers;
using System.Xml.Linq;
using ErrorListeners;
using Tools;
using Models;
using Models.Domain;
using Models.Problem;
using Models.AST;
using ASTGenerator;
using Contextualisers;

namespace Parsers
{
    public class PDDLParser : IPDDLParser
    {
        public IErrorListener Listener { get; }
        public bool Contextualise { get; set; }
        public bool Analyse { get; set; }

        public PDDLParser(bool contextualise = true, bool analyse = true)
        {
            Listener = new ErrorListener();
            Listener.ThrowIfTypeAbove = ParseErrorType.Warning;
            Contextualise = contextualise;
            Analyse = analyse;
        }

        public PDDLDecl TryParse(string domainFile = null, string problemFile = null)
        {
            try
            {
                return Parse(domainFile, problemFile);
            }
            catch { return new PDDLDecl(null, null); }
        }

        public PDDLDecl Parse(string domainFile = null, string problemFile = null)
        {
            var decl = new PDDLDecl(
                ParseDomain(domainFile),
                ParseProblem(problemFile));

            if (Contextualise)
            {
                if (domainFile != null && problemFile != null)
                {
                    IContextualiser<PDDLDecl> contextualiser = new PDDLDeclContextualiser();
                    contextualiser.Contexturalise(decl, Listener);
                }
                else if (domainFile != null)
                {
                    IContextualiser<DomainDecl> contextualiser = new PDDLDomainDeclContextualiser();
                    contextualiser.Contexturalise(decl.Domain, Listener);
                }
                else if (problemFile != null)
                {
                    IContextualiser<ProblemDecl> contextualiser = new PDDLProblemDeclContextualiser();
                    contextualiser.Contexturalise(decl.Problem, Listener);
                }
            }

            if (Analyse)
            {
                if (domainFile != null && problemFile != null)
                {
                    IAnalyser<PDDLDecl> pddlAnalyser = new PDDLDeclAnalyser();
                    pddlAnalyser.PostAnalyse(decl, Listener);
                }
                else if (domainFile != null)
                {
                    IAnalyser<DomainDecl> domainAnalyser = new PDDLDomainDeclAnalyser();
                    domainAnalyser.PostAnalyse(decl.Domain, Listener);
                }
                else if (problemFile != null)
                {
                    IAnalyser<ProblemDecl> problemAnalyser = new PDDLProblemDeclAnalyser();
                    problemAnalyser.PostAnalyse(decl.Problem, Listener);
                }
            }


            return decl;
        }

        private DomainDecl ParseDomain(string parseFile)
        {
            if (parseFile == null)
                return new DomainDecl(new ASTNode());

            if (!PDDLFileHelper.IsFileDomain(parseFile))
                Listener.AddError(new ParseError(
                    $"Attempted file to parse was not a domain file!",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing,
                    ParserErrorCode.FileNotDomain));

            var absAST = ParseAsASTTree(parseFile, Listener);

            IVisitor<ASTNode, INode, IDecl> visitor = new DomainVisitor();
            var returnDomain = visitor.Visit(absAST, null, Listener);
            return returnDomain as DomainDecl;
        }

        private ProblemDecl ParseProblem(string parseFile)
        {
            if (parseFile == null)
                return new ProblemDecl(new ASTNode());

            if (!PDDLFileHelper.IsFileProblem(parseFile))
                Listener.AddError(new ParseError(
                    $"Attempted file to parse was not a problem file!",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing,
                    ParserErrorCode.FileNotProblem));

            var absAST = ParseAsASTTree(parseFile, Listener);

            IVisitor<ASTNode, INode, IDecl> visitor = new ProblemVisitor();
            var returnProblem = visitor.Visit(absAST, null, Listener);
            return returnProblem as ProblemDecl;
        }

        private ASTNode ParseAsASTTree(string path, IErrorListener listener)
        {
            var text = ReadDataAsString(path, listener);
            var analyser = new GeneralPreAnalyser();
            analyser.PreAnalyse(text, listener);

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
                    ParseErrorLevel.PreParsing,
                    ParserErrorCode.FileNotFound));
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
