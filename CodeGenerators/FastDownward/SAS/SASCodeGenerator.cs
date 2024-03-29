﻿using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.FastDownward.SAS;

namespace PDDLSharp.CodeGenerators.FastDownward.SAS
{
    public class SASCodeGenerator : ICodeGenerator<ISASNode>
    {
        public IErrorListener Listener { get; }
        public bool Readable { get; set; } = false;

        public SASCodeGenerator(IErrorListener listener)
        {
            Listener = listener;
        }

        public void Generate(ISASNode node, string toFile) => File.WriteAllText(toFile, Generate(node));
        public string Generate(ISASNode node)
        {
            SectionVisitor visitor = new SectionVisitor();
            var retStr = "";
            try
            {
                retStr = visitor.Visit((dynamic)node);
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
