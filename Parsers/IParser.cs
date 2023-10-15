using PDDLSharp.ErrorListeners;

namespace PDDLSharp.Parsers
{
    public interface IParser<T>
    {
        public IErrorListener Listener { get; }

        public T Parse(FileInfo file);
        public T Parse(string text);
        public U ParseAs<U>(FileInfo file) where U : T;
        public U ParseAs<U>(string text) where U : T;
    }
}
