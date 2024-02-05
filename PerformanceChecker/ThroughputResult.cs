using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceChecker
{
    public class ThroughputResult
    {
        public string Name { get; set; }
        public string MeanTime => $"{_meanTime.TotalMicroseconds} μs";
        private TimeSpan _meanTime;
        public string Size => $"{_lengthBytes} B";
        public double Throughput => Math.Round(((double)_lengthBytes / 1000000) / _meanTime.TotalSeconds, 2);
        private int _lengthBytes;

        public ThroughputResult(string name, TimeSpan meanTime, int lengthBytes)
        {
            Name = name;
            _meanTime = meanTime;
            _lengthBytes = lengthBytes;
        }
    }
}
