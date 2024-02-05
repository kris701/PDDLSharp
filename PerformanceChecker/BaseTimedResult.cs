using System.Diagnostics;

namespace PerformanceChecker
{
    internal abstract class BaseTimedResult
    {
        internal long TimeMs { get; private set; }
        public double TimeS => Math.Round((double)TimeMs / 1000, 3);
        private Stopwatch _watch = new Stopwatch();

        public void Start()
        {
            _watch.Start();
        }

        public void Stop()
        {
            _watch.Stop();
            TimeMs = _watch.ElapsedMilliseconds;
        }
    }
}
