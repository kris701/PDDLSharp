using PDDLSharp.CodeGenerators.Visitors;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.CodeGenerators
{
    public class PDDLCodeGenerator : ICodeGenerator<INode>
    {
        public IErrorListener Listener { get; }
        public bool Readable { get; set; } = false;

        public PDDLCodeGenerator(IErrorListener listener)
        {
            Listener = listener;
        }

        public void Generate(INode node, string toFile) => File.WriteAllText(toFile, Generate(node));
        public string Generate(INode node)
        {
            GeneratorVisitors visitor = new GeneratorVisitors(Readable);
            var retStr = "";
            try
            {
                retStr = visitor.Visit((dynamic)node, 0);
                while (retStr.Contains($"{Environment.NewLine}{Environment.NewLine}"))
                    retStr = retStr.Replace($"{Environment.NewLine}{Environment.NewLine}", Environment.NewLine);
            }
            catch (ParseException e)
            {

            }
            catch (Exception e)
            {
                Listener.AddError(new ParseError(
                    $"Unexpected exception occured during code generation: {e.Message}",
                    ParseErrorType.Error,
                    ParseErrorLevel.CodeGeneration));
            }
            return retStr;
        }
    }
}
