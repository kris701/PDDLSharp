using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp
{
    public class Options
    {
        // General
        [Option("domain", Required = true, HelpText = "Path to the benchmark file to use")]
        public string Domain { get; set; } = "";
    }
}
