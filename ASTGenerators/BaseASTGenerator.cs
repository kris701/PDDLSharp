using PDDLSharp.ASTGenerators.SAS;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ASTGenerators
{
    public abstract class BaseASTGenerator : IGenerator
    {
        public IErrorListener Listener { get; }

        public BaseASTGenerator(IErrorListener listener)
        {
            Listener = listener;
        }

        public ASTNode Generate(FileInfo file) => Generate(File.ReadAllText(file.FullName));
        public abstract ASTNode Generate(string text);

        public static List<int> GenerateLineDict(string source, char breakToken)
        {
            List<int> lineDict = new List<int>();
            int offset = source.IndexOf(breakToken);
            while (offset != -1)
            {
                lineDict.Add(offset);
                offset = source.IndexOf(breakToken, offset + 1);
            }
            return lineDict;
        }

        public static int GetLineNumber(List<int> lineDict, int start, int offset)
        {
            int length = lineDict.Count;
            for (int i = offset; i < length; i++)
                if (start < lineDict[i])
                    return i + 1;
            return lineDict.Count + 1;
        }
    }
}
