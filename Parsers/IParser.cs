using PDDLSharp.ErrorListeners;

namespace PDDLSharp.Parsers
{
    public interface IParser<T>
    {
        public IErrorListener Listener { get; }

        public T Parse(string file);
        public U ParseAs<U>(string file) where U : T;
    }
}
