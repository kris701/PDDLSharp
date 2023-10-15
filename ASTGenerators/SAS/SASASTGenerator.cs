using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ASTGenerators.SAS
{
    public class SASASTGenerator : IGenerator
    {
        public IErrorListener Listener { get; }

        public SASASTGenerator(IErrorListener listener)
        {
            Listener = listener;
        }

        public ASTNode Generate(FileInfo file) => Generate(File.ReadAllText(file.FullName));

        public ASTNode Generate(string text)
        {
            var returnNode = new ASTNode(0, text.Length, text, text);
            int offset = 0;
            while (offset != -1)
            {
                var begin = text.IndexOf("begin_", offset);
                var end = text.IndexOf("end_", offset);
                var areaText = text.Substring(begin, end - begin);
                returnNode.Children.Add(new ASTNode(
                    begin,
                    end,
                    areaText,
                    areaText
                    ));
            }
            return returnNode;
        }
    }
}
