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
    public class InitialPerfectVerbs: IDisposable
    {
        private string fileName;

        public InitialPerfectVerbs(string fileName)
        {
            this.fileName = fileName;
        }

        public Dictionary<string, string> GetVerbsPairs()
        {
            var phrasesWithVerbPairs = GetPhrasesWithVerbsPairs();

            var analyzer = MorphAnalyzer.Instance.Analyzer;
            var verbPairs = phrasesWithVerbPairs.Select(l => l.Where(x =>
            {
                foreach (var verb in analyzer.parse(x))
                {
                    string tags = (verb.tag).ToString();
                    if (tags.Contains("INFN") || tags.Contains("VERB") || tags.Contains("UNKN"))
                        return true;
                }
                return false;
            }));

            var initialPerfectDictionary = new Dictionary<string, string>();

            foreach(var verb in verbPairs)
            {
                if (!initialPerfectDictionary.ContainsKey(verb.ElementAt(1)))
                {
                    initialPerfectDictionary.Add(verb.ElementAt(1), verb.ElementAt(0));
                }
            }

            return initialPerfectDictionary;
        }

        private static string DeleteNumbers(string str) => Regex.Replace(str, "[0-9]", "", RegexOptions.IgnoreCase);
        
        private static string DeleteTranistivity(string str) => Regex.Replace(str, "(сь|ся)", "", RegexOptions.IgnoreCase);

        private static IEnumerable<IEnumerable<string>> DeleteEmptyEntries(IEnumerable<string> phrases) => 
            phrases.Select(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        private IEnumerable<IEnumerable<string>> GetPhrasesWithVerbsPairs()
        {
            var fileText = System.IO.File.ReadAllLines(this.fileName, Encoding.UTF8);

            var verbPairsWithoutNumbers = fileText.Select(x => DeleteNumbers(x));
            var verbPairsWithoutTransitivity = verbPairsWithoutNumbers.Select(x => DeleteTranistivity(x));

            var verbPairsWithoutNumbersList = DeleteEmptyEntries(verbPairsWithoutNumbers);
            var verbPairsWithoutTransitivityList = DeleteEmptyEntries(verbPairsWithoutTransitivity);

            var verbsPairs = new HashSet<IEnumerable<string>>(new SequentialStringComparer());
            foreach(var verb in verbPairsWithoutNumbersList)
            {
                verbsPairs.Add(verb);
            }
            foreach (var verb in verbPairsWithoutTransitivityList)
            {
                verbsPairs.Add(verb);
            }
            return verbsPairs;
        }

        public void Dispose()
        {
            MorphAnalyzer.Instance.Dispose();
        }
    }
}
