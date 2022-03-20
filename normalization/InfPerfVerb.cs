using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Python.Included;
using Python.Runtime;

namespace normalization
{
    public class InfPerfVerb
    {
        private Dictionary<string, string> _PerfectVerbs = new Dictionary<string, string>();

        async public void FullDictionary(string fileName)
        {
            Installer.InstallPath = Path.GetFullPath(".");
            await Installer.SetupPython();

            PythonEngine.Initialize();

            if (!Installer.IsModuleInstalled("pymorphy2"))
            {
                Installer.TryInstallPip();
                Installer.PipInstallModule("pymorphy2");
            }

            dynamic pm = Py.Import("pymorphy2");
            dynamic morph = pm.MorphAnalyzer();
            //PyModule scope = Py.CreateScope();
            //string result = morph.parse("начать")[0].tag;
            //Console.WriteLine(scope.Exec(""));
            
            _PerfectVerbs.Clear();

            string[] lines = System.IO.File.ReadAllLines(fileName, Encoding.UTF8);

            var linesWithoutTranAndNum = lines.Select(l => DeleteNumbersAndTrans(l).Split(' ', StringSplitOptions.RemoveEmptyEntries))
                                   .ToList();

            var resultLines = linesWithoutTranAndNum.Select(l => l.Select(x => x)
                                                                  .Where(x => 
                                                                  {
                                                                      foreach(var i in morph.parse(x))
                                                                      {
                                                                          string r = (i.tag).ToString();
                                                                          if(r.Contains("INFN") || r.Contains("VERB") || r.Contains("UNKN")) return true;
                                                                      }
                                                                      return false;
                                                                   })
                                                                  .ToList())
                                                    .ToList();

            foreach(var line in resultLines)
            {
                if (!_PerfectVerbs.ContainsKey(line[0]))
                {
                    _PerfectVerbs.Add(line[0], line[1]);
                }
         
            }
            PythonEngine.Shutdown();
        }

        private string DeleteNumbersAndTrans(string str)
        {
            str = Regex.Replace(str, "[0-9]", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "(сь|ся)", "", RegexOptions.IgnoreCase);
            return str;
        }

        public Dictionary<string, string> PerfectVerbs { get => _PerfectVerbs; }

    }
}
