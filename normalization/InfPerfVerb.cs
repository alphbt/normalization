using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace normalization
{
    public class InfPerfVerb
    {
        private Dictionary<string, HashSet<string>> _PerfectVerbs = new Dictionary<string, HashSet<string>>();

        public void FullDictionary(string fileName)
        {
            _PerfectVerbs.Clear();

            string[] lines = System.IO.File.ReadAllLines(fileName, Encoding.UTF8);

            var resultLines = lines.Select(l => DeleteNumbersAndTrans(l).Split(new char[] { ' ' })).ToList();

            foreach(var line in resultLines)
            {
                if (_PerfectVerbs.ContainsKey(line[0]))
                {
                    _PerfectVerbs[line[0]].Add(line[1]);
                }
                else
                {
                    _PerfectVerbs.Add(line[0], new HashSet<string>());
                    _PerfectVerbs[line[0]].Add(line[1]);
                }
            }
        }

        private string DeleteNumbersAndTrans(string str)
        {
            str = Regex.Replace(str, "[0-9]", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "(сь|ся)", "", RegexOptions.IgnoreCase);
            return str;
        }

        public Dictionary<string, HashSet<string>> PerfectVerbs { get => _PerfectVerbs; }

    }
}
