using PDDLSharp.CodeGenerators.Visitors;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;

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
            catch (PDDLSharpException)
            {

            }
            catch (Exception e)
            {
                Listener.AddError(new PDDLSharpError(
                    $"Unexpected exception occured during code generation: {e.Message}",
                    ParseErrorType.Error,
                    ParseErrorLevel.CodeGeneration));
            }
            return retStr;
        }
    }
}
