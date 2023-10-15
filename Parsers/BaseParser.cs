using PDDLSharp.ErrorListeners;

namespace PDDLSharp.Parsers
{
    public abstract class BaseParser<T> : IParser<T>
    {
        public IErrorListener Listener { get; }

        public BaseParser(IErrorListener listener)
        {
            Listener = listener;
        }

        public T Parse(FileInfo file) => Parse(File.ReadAllText(file.FullName));
        public T Parse(string text) => ParseAs<T>(text);
        public U ParseAs<U>(FileInfo file) where U : T => ParseAs<U>(File.ReadAllText(file.FullName));
        public abstract U ParseAs<U>(string text) where U : T;
    }
}
