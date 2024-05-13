using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;

namespace PDDLSharp.ASTGenerators
{
    public abstract class BaseASTGenerator : IGenerator
    {
        public IErrorListener Listener { get; }
        public bool SaveLinePlacements { get; set; } = false;

        public BaseASTGenerator(IErrorListener listener)
        {
            Listener = listener;
        }

        public ASTNode Generate(FileInfo file) => Generate(File.ReadAllText(file.FullName));
        public abstract ASTNode Generate(string text);

        public List<int> GenerateLineDict(string source, char breakToken)
        {
            if (!SaveLinePlacements)
                return new List<int>();
            List<int> lineDict = new List<int>();
            int offset = source.IndexOf(breakToken);
            while (offset != -1)
            {
                lineDict.Add(offset);
                offset = source.IndexOf(breakToken, offset + 1);
            }
            return lineDict;
        }

        public int GetLineNumber(List<int> lineDict, int start, int offset)
        {
            if (!SaveLinePlacements)
                return -1;
            int length = lineDict.Count;
            for (int i = offset; i < length; i++)
                if (start < lineDict[i])
                    return i + 1;
            return lineDict.Count + 1;
        }
    }
}
