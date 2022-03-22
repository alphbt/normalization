using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Python.Included;
using Python.Runtime;

namespace normalization
{
    public class CrossLex
    {
        private List<List<string>> words = new List<List<string>>();

        async public void FullList(string fileName, Dictionary<string, string> perfPairs)
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

            var word = File.ReadLines(fileName, Encoding.UTF8)
                           .TakeWhile(l => l.Count() != 0)
                           .Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                           .ToList()[0][1];

            //var lines = new List<string>();
            var hasPredicates = File.ReadLines(fileName, Encoding.UTF8)
                                    .SkipWhile(l => !l.Contains("Section: Has Predicates"))
                                    .Skip(2)
                                    .TakeWhile(l => l.Count() != 0); ;
           

            var governedByVerb = File.ReadLines(fileName, Encoding.UTF8)
                                     .SkipWhile(l => !l.Contains("Section: Governed by Verbs"))
                                     .Skip(2)
                                     .TakeWhile(l => l.Count() != 0);

            var tags = new List<string>() { "коллок", "локал", "книжн", "вульг", "неправ", "mb", "fig", "идиом", "и", "прямое" };

            var lines1 = governedByVerb.Select(l => l.Split(new char[] {' ','.','(', ')','/'}, StringSplitOptions.RemoveEmptyEntries)).Where(l => l.Count() != 0);
            var lines2 = lines1.Select(l => l.Select(x => x).Where(x => !tags.Contains(x) && !x.Equals(word)));
            var lines3 = lines2.Select(l => l.Select(x => x).Where(x =>
            {
                foreach (var i in morph.parse(x))
                {
                    string r = (i.tag).ToString();
                    if (r.Contains("INFN") || r.Contains("VERB") || r.Contains("PREP")) return true;
                }
                return false;
            }));
            var lines4 = lines3.Select(l => l.Select(x => {
                                                                string res = "";
                                                                foreach (var i in morph.parse(x))
                                                                {
                                                                    string r = (i.tag).ToString();
                                                                    if (r.Contains("INFN") || r.Contains("VERB"))
                                                                        res = Regex.Replace((i.normal_form).ToString(), "(сь|ся)", "", RegexOptions.IgnoreCase);
                                                                    else if (r.Contains("PREP")) res = x;      
                                                                }
                                                                return res;
                                                           }).Where(x => !x.Equals("")).Distinct());

            words = lines4.Select(l => l.Select(x => perfPairs.ContainsKey(x) ? perfPairs[x] : x).ToList()).ToList();
            PythonEngine.Shutdown();
        }
    }
}
