using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Python.Included;
using Python.Runtime;
using ServiceStack.Text;
using ServiceStack;

namespace Normalization
{
    public class InitialPerfectVerbs
    {
        public static Dictionary<string, string> GetVerbsPairs(string fileName)
        {
            
            using (Py.GIL())
            {
                var phrasesWithVerbPairs = GetPhrasesWithVerbsPairs(fileName);

                var analyzer = MorphAnalyzer.Instance.Analyzer;
                var verbPairs = phrasesWithVerbPairs.Select(l => l.Where(x =>
                {
                    var parsedData = analyzer.parse(x);
                    foreach (var verb in parsedData)
                    {
                        string tags = (verb.tag).ToString();
                        if (tags.Contains("INFN") || tags.Contains("VERB") || tags.Contains("UNKN"))
                            return true;
                    }
                    return false;
                }));


                var initialPerfectDictionary = new Dictionary<string, string>();

                foreach (var verb in verbPairs)
                {
                    if (!initialPerfectDictionary.ContainsKey(verb.ElementAt(1)))
                    {
                        initialPerfectDictionary.Add(verb.ElementAt(1), verb.ElementAt(0));
                    }
                }

                return initialPerfectDictionary;
            }
        }

        private static string DeleteNumbers(string str) => Regex.Replace(str, "[0-9]", "", RegexOptions.IgnoreCase);

        private static string DeleteTranistivity(string str)
        {
            var verb = Regex.Replace(str, "(сь|ся)", "", RegexOptions.IgnoreCase);
            dynamic parsed;

            using (Py.GIL())
            {
                var analyzer = MorphAnalyzer.Instance.Analyzer;

                parsed = (analyzer.parse(verb)[0]);
                var tag = (parsed.tag).ToString();

                if (tag.Contains("intr"))
                {
                    return str;
                }

                return verb;
            }
            
        }

        private static IEnumerable<IEnumerable<string>> DeleteEmptyEntries(IEnumerable<string> phrases) => 
            phrases.Select(x => x.Split(new [] {' ', ','}, StringSplitOptions.RemoveEmptyEntries));

        private static IEnumerable<IEnumerable<string>> GetPhrasesWithVerbsPairs(string fileName)
        {
            var fileText = System.IO.File.ReadAllLines(fileName, Encoding.UTF8);

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

        public static Dictionary<string, string> ReadVerbPairsFromCsv(string csvPath)
        {
            var verbPairs = new Dictionary<string, string>();

            var deserialized = File.ReadAllText(csvPath).FromCsv<List<string>>();
            var rslt = DeleteEmptyEntries(deserialized.Skip(1));
            
            foreach(var e in rslt)
            {
                if(!verbPairs.ContainsKey(e.ElementAt(0)))
                    verbPairs.Add(e.ElementAt(0), e.ElementAt(1));
            }

            return verbPairs;
        }
    }
}
