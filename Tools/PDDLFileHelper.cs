using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Tools
{
    public static class PDDLFileHelper
    {
        public static bool IsFileDomain(string path)
        {
            if (path == null)
                return false;
            if (!File.Exists(path))
                return false;
            var info = new FileInfo(path);
            if (info.Exists && info.Extension.ToLower() == ".pddl")
            {
                var fileText = File.ReadAllText(info.FullName);
                if (fileText.ToUpper().Contains("(DOMAIN "))
                    return true;
            }
            return false;
        }

        public static bool IsFileProblem(string path)
        {
            if (path == null)
                return false;
            if (!File.Exists(path))
                return false;
            var info = new FileInfo(path);
            if (info.Exists && info.Extension.ToLower() == ".pddl")
            {
                var fileText = File.ReadAllText(info.FullName);
                if (fileText.ToUpper().Contains("(PROBLEM "))
                    return true;
            }
            return false;
        }
    }
}
