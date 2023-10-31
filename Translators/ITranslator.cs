namespace PDDLSharp.Translators
{
    public interface ITranslator<From, To>
    {
        public To Translate(From from);

        public TimeSpan TimeLimit { get; set; }
        public TimeSpan TranslationTime { get; }

        public bool Aborted { get; }
    }
}
