using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorListeners
{
    public enum ParserErrorCode { 
        None,
        ParenthesesMissmatch,
        UpperCaseLettersAreIgnored,
        UnsupportedPackagesUsed,

        UseOfUndeclaredPredicate,
        UseOfUndeclaredObject,
        InvalidObjectType,
        InvalidPredicateType,

        MissingItem,

        UnusedPredicate,
        UnusedParameter,
        UnusedObject,

        MultipleDeclarationsOfType,
        MultipleDeclarationsOfSuperType,
        MultipleDeclarationsOfSubType,
        MultipleDeclarationsOfPredicate,
        MultipleDeclarationsOfParameter,
        MultipleDeclarationsOfActions,
        MultipleDeclarationsOfObjects,

        UnknownType,
        TypeMissmatch,

        NoGoalsDeclared,

        FileNotFound,
        FileNotDomain,
        FileNotProblem,

        StrayCharactersFound,
        NoChildrenAllowed,
        NeedExactChildren,
        NeedExactLooseChildren,
        MustHaveMoreThanChildren,
        UnexpectedNodeType,
        CouldNotParsePredicate,
        CouldNotParseName,

        UnknownNode,
        ExpectedTypeButGotNone,
        ExpectedNameButGotNone,

        TypeDeclarationError,
    }

    public enum ParseErrorType { None, Message, Warning, Error }
    public enum ParseErrorLevel { None, PreParsing, Parsing, Contexturaliser, Analyser }
    public class ParseError
    {
        public ParserErrorCode Code { get; internal set; }
        public string Message { get; internal set; }
        public ParseErrorType Type { get; internal set; }
        public ParseErrorLevel Level { get; internal set; }
        public int Line { get; internal set; }
        public int Character { get; internal set; }

        public ParseError(string message, ParseErrorType type, ParseErrorLevel level, ParserErrorCode code, int line, int character)
        {
            Message = message;
            Type = type;
            Level = level;
            Line = line;
            Character = character;
            Code = code;
        }

        public ParseError(string message, ParseErrorType type, ParseErrorLevel level, ParserErrorCode code)
        {
            Message = message;
            Type = type;
            Level = level;
            Line = -1;
            Character = -1;
            Code = code;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case ParseErrorType.None:
                    return $"       {Message}";
                case ParseErrorType.Message:
                    return $"[INFO] {Message}";
                case ParseErrorType.Warning:
                    return $"[WARN] {Message}";
                case ParseErrorType.Error:
                    return $"[ERRO] {Message}";
            }
            return Message;
        }
    }
}
