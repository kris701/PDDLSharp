namespace PDDLSharp.Tools
{
    public static class StringHelpers
    {
        // Faster string replacement
        // From https://stackoverflow.com/a/54056154
        public static string ReplaceRangeWithSpacesFast(string text, int from, int to)
        {
            int length = to - from;
            string replacement = new string(' ', to - from);
            return string.Create(text.Length - length + replacement.Length, (text, from, length, replacement),
                (span, state) =>
                {
                    state.text.AsSpan().Slice(0, state.from).CopyTo(span);
                    state.replacement.AsSpan().CopyTo(span.Slice(state.from));
                    state.text.AsSpan().Slice(state.from + state.length).CopyTo(span.Slice(state.from + state.replacement.Length));
                });
        }
    }
}
