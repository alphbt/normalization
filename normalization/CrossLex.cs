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

        static IEnumerable<string> GetInfVerbForm(IEnumerable<string> enumerable)
        {
            return enumerable.Select(l =>
            {
                string normalForm = "";

                foreach (var i in MorphAnalyzer.Instance.Analyzer.parse(l))
                {
                    string r = (i.tag).ToString();
                    if (r.Contains("INFN") || r.Contains("VERB"))
                    {
                        normalForm = (i.normal_form).ToString();
                        break;
                    }
                }
                return normalForm.Equals("") ? l : normalForm;
            });
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

        public IDictionary<IEnumerable<string>, HashSet<IEnumerable<string>>> GetDictionaryOfInitialForms()
        {
            var infForms = new Dictionary<IEnumerable<string>, HashSet<IEnumerable<string>>>(new SequentialStringComparer());
            var combinations = GetCombinations();
            foreach (var comb in combinations)
            {
                var infVerbForm = GetInfVerbForm(comb);
                if (!infForms.ContainsKey(infVerbForm))
                {
                    infForms.Add(infVerbForm, new HashSet<IEnumerable<string>>(new SequentialStringComparer()));
                }
                infForms[infVerbForm].Add(comb);
            }
            return infForms;
        }

        public IDictionary<IEnumerable<string>, HashSet<IEnumerable<string>>> MixVerbs(IDictionary<string, string> perfectVerbsDict)
        {
            var dict = GetDictionaryOfInitialForms();
            var normalizedForms = new Dictionary<IEnumerable<string>, HashSet<IEnumerable<string>>>(new SequentialStringComparer());
            var initialVerbsForm = dict.Keys;

            foreach(var verb in initialVerbsForm)
            {
                var normalizeVerb = verb.ImperfectIntransitiveRule()
                    .PerfectIntransiveRule(perfectVerbsDict).PerfectIntransiveWithoutEndRule(perfectVerbsDict)
                    .PerfectTransitiveRule(perfectVerbsDict).ToList();

                if(!normalizedForms.ContainsKey(normalizeVerb))
                {
                    normalizedForms.Add(normalizeVerb, new HashSet<IEnumerable<string>>(new SequentialStringComparer()));
                }
                normalizedForms[normalizeVerb].Add(verb.ToList());
            }
            return normalizedForms;
        }

        public void Dispose()
        {
            MorphAnalyzer.Instance.Dispose();
        }
    }
}
