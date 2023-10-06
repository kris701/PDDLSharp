namespace PDDLSharp.ErrorListeners
{
    public enum ParseErrorType { None, Message, Warning, Error }
    public enum ParseErrorLevel { None, PreParsing, Parsing, Contexturaliser, Analyser, CodeGeneration }
    public class PDDLSharpError
    {
        public string Message { get; internal set; }
        public ParseErrorType Type { get; internal set; }
        public ParseErrorLevel Level { get; internal set; }
        public int Line { get; internal set; }
        public int Character { get; internal set; }

        public PDDLSharpError(string message, ParseErrorType type, ParseErrorLevel level, int line, int character)
        {
            Message = message;
            Type = type;
            Level = level;
            Line = line;
            Character = character;
        }

        public PDDLSharpError(string message, ParseErrorType type, ParseErrorLevel level)
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
                    return $"[NONE] [{Enum.GetName(typeof(ParseErrorLevel), Level)}] {Message}";
                case ParseErrorType.Message:
                    return $"[INFO] [{Enum.GetName(typeof(ParseErrorLevel), Level)}] {Message}";
                case ParseErrorType.Warning:
                    return $"[WARN] [{Enum.GetName(typeof(ParseErrorLevel), Level)}] {Message}";
                case ParseErrorType.Error:
                    return $"[ERRO] [{Enum.GetName(typeof(ParseErrorLevel), Level)}] {Message}";
            }
            return Message;
        }
    }
}
