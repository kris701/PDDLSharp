using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ErrorListeners
{
    public enum ParseErrorType { None, Message, Warning, Error }
    public enum ParseErrorLevel { None, PreParsing, Parsing, Contexturaliser, Analyser, CodeGeneration }
    public class ParseError
    {
        public string Message { get; internal set; }
        public ParseErrorType Type { get; internal set; }
        public ParseErrorLevel Level { get; internal set; }
        public int Line { get; internal set; }
        public int Character { get; internal set; }

        public ParseError(string message, ParseErrorType type, ParseErrorLevel level, int line, int character)
        {
            Message = message;
            Type = type;
            Level = level;
            Line = line;
            Character = character;
        }

        public ParseError(string message, ParseErrorType type, ParseErrorLevel level)
        {
            Message = message;
            Type = type;
            Level = level;
            Line = -1;
            Character = -1;
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
