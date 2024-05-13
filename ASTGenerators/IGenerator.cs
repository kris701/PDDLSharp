using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;

namespace PDDLSharp.ASTGenerators
{
    public interface IGenerator
    {
        public IErrorListener Listener { get; }
        public bool SaveLinePlacements { get; set; }

        public ASTNode Generate(FileInfo file);
        public ASTNode Generate(string text);
    }
}
