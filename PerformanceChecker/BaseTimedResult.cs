using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceChecker
{
    internal abstract class BaseTimedResult
    {
        internal long TimeMs { get; private set; }
        public double TimeS => Math.Round((double)TimeMs / 1000, 2);
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
