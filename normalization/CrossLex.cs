using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nito.AsyncEx.Synchronous;
using Python.Included;
using Python.Runtime;
using static MoreLinq.Extensions.InsertExtension;
using static MoreLinq.Extensions.BacksertExtension;


namespace normalization
{
    public class CrossLex : IDisposable
    {
        private string fileName;
        static private List<string> tags = new List<string>() { "коллок", "локал", "книжн", "вульг", "неправ", "mb", "fig", "идиом", "и", "прямое", "не" };

        public CrossLex(string fileName)
        {
            this.fileName = fileName;
        }

        static IEnumerable<IEnumerable<string>> SplitString(IEnumerable<string> l) =>
                l.Select(x => x.Split(new char[] { ' ', '.', '(', ')', '/' }, StringSplitOptions.RemoveEmptyEntries))
                               .Where(x => x.Count() != 0);

        private IEnumerable<string> ReadFromFile(string crossLexSection) =>
                                    File.ReadLines(this.fileName, Encoding.UTF8)
                                    .SkipWhile(l => !l.Contains(crossLexSection))
                                    .Skip(2)
                                    .TakeWhile(l => l.Count() != 0);

        static IEnumerable<IEnumerable<string>> RemoveTags(IEnumerable<IEnumerable<string>> l) =>
            l.Select(x => x.Select(y => y).Where(y => !tags.Contains(y)));

        static VerbInfo GetInitialVerbForm(IEnumerable<string> enumerable)
        {
            return new VerbInfo()
            {
                Verb = enumerable.ElementAt(0),
                Prep = enumerable.Skip(1).Count() == 0 ? "" : enumerable.Skip(1).Aggregate((x, y) => x + " " + y)
            };            
        }

        static IEnumerable<IEnumerable<string>> RemoveMainNoun(IEnumerable<IEnumerable<string>> enumerable, string noun) =>
                        enumerable.Select(l => l.Select(x => x).Where(x =>
                        {
                            foreach (var i in MorphAnalyzer.Instance.Analyzer.parse(x))
                            {
                                string r = (i.normal_form).ToString();
                                if (r.Equals(noun)) return false;
                            }
                            return true;
                        }));

        private IEnumerable<IEnumerable<string>> GetCombinationsBySection(string section)
        {
            var noun = Regex.Replace(Path.GetFileName(this.fileName), ".txt", "", RegexOptions.IgnoreCase).ToLower();
            var combintaions = ReadFromFile(section);
            var splitingCombinations = SplitString(combintaions);
            var withoutTagsCombinations = RemoveTags(splitingCombinations);
            var withoutMainNounCombinations = RemoveMainNoun(withoutTagsCombinations, noun);
            return EnumerableExtensions.If(withoutMainNounCombinations, x => x.Select(l => l.Take(1)), section == "Section: Has Predicates");
        }

        private IEnumerable<IEnumerable<string>> GetCombinations() =>
            GetCombinationsBySection("Section: Has Predicates")
            .Backsert(GetCombinationsBySection("Section: Governed by Verbs"), 0);

        public IDictionary<VerbInfo, HashSet<IEnumerable<string>>> GetDictionaryOfInitialForms()
        {
            var initialVerbsDict = new Dictionary<VerbInfo, HashSet<IEnumerable<string>>>(new VerbsComparer());
            var combinations = GetCombinations();

            foreach (var comb in combinations)
            {
                var initialVerb = GetInitialVerbForm(comb);
                if (!initialVerbsDict.ContainsKey(initialVerb))
                {
                    initialVerbsDict.Add(initialVerb, new HashSet<IEnumerable<string>>(new SequentialStringComparer()));
                }
                initialVerbsDict[initialVerb].Add(comb);
            }

            return initialVerbsDict;
        }

        public Dictionary<VerbInfo, HashSet<VerbInfo>> MixVerbs(IDictionary<string, string> perfectVerbsDict)
        {
            var initialVerbsFormDict = GetDictionaryOfInitialForms();           
            var initialVerbsForm = initialVerbsFormDict.Keys;

            var normalizedForms = new Dictionary<VerbInfo, HashSet<VerbInfo>>(new VerbsComparer());
            normalizedForms = initialVerbsForm.CompressVerb<VerbInfo>(perfectVerbsDict);

            return normalizedForms;
        }

        public void Dispose()
        {
            MorphAnalyzer.Instance.Dispose();
        }
    }
}
