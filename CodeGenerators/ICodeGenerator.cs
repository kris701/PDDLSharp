using PDDLSharp.ErrorListeners;

namespace PDDLSharp.CodeGenerators
{
    public interface ICodeGenerator<T>
    {
        public IErrorListener Listener { get; }
        public bool Readable { get; set; }

        public string Generate(T node);
        public void Generate(T node, string toFile);
    }
}
