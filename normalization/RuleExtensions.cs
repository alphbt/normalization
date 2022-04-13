using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static MoreLinq.Extensions.BacksertExtension;


namespace normalization
{
    public static class RuleExtensions
    {
        private static dynamic? GetAnalyzerResult(IEnumerable<string> items)
        {
            var verb = items.ElementAt(0);
            return MorphAnalyzer.Instance.Analyzer.parse(verb);
        }

        private static bool IsMatching(string tags, params string[] tagNames)
        {
            return tagNames.All(x => tags.Contains(x));
        }

        public static IEnumerable<string> PerfectTransitiveRule(this IEnumerable<string> items, IDictionary<string, string> perfectVerbsDict)
        {
            var analyzerResult = GetAnalyzerResult(items);
            var verb = items.ElementAt(0);

            foreach(var parsedVerb in analyzerResult!)
            {
                var tag = (parsedVerb.tag).ToString();
                if(IsMatching(tag, "perf", "tran") && perfectVerbsDict.ContainsKey(verb))
                {
                    return (new[] { perfectVerbsDict[verb] }).Backsert(items.Skip(1), 0);  
                }
            }
            return items;
        }

        public static IEnumerable<string> PerfectIntransiveWithoutEndRule(this IEnumerable<string> items, IDictionary<string, string> perfVerbsDict)
        {
            var analyzerResult = GetAnalyzerResult(items);
            var verb = items.ElementAt(0);
            var rgx = new Regex(@"\w*(сь|ся)$");

            foreach (var parsedVerb in analyzerResult!)
            {
                var tag = (parsedVerb.tag).ToString();
                if(IsMatching(tag, "perf", "intr") && !rgx.IsMatch(items.ElementAt(0)) && perfVerbsDict.ContainsKey(verb))
                {
                    return (new[] { perfVerbsDict[verb] }).Backsert(items.Skip(1), 0);  
                }
            }
            return items;
        }

        public static IEnumerable<string>  PerfectIntransiveRule(this IEnumerable<string> items, IDictionary<string, string> perfVerbsDict)
        {
            var analyzerResult = GetAnalyzerResult(items);
            var verb = items.ElementAt(0);
            var rgx = new Regex(@"(сь|ся)$");

            foreach (var parsedVerb in analyzerResult!)
            {
                var tag = (parsedVerb.tag).ToString();
                if (IsMatching(tag, "perf", "intr") && rgx.IsMatch(verb))
                {
                    verb = rgx.Replace(verb, "");
                    if(perfVerbsDict.ContainsKey(verb))
                        return (new[] { perfVerbsDict[verb] }).Backsert(items.Skip(1), 0);
                    else 
                        return (new[] {verb}).Backsert(items.Skip(1), 0);
                }
            }
            return items;
        }

        public static IEnumerable<string> ImperfectIntransitiveRule(this IEnumerable<string> items)
        {
            var analyzerResult = GetAnalyzerResult(items);
            var verb = items.ElementAt(0);
            var rgx = new Regex(@"(сь|ся)$");

            foreach (var parsedVerb in analyzerResult!)
            {
                var tag = (parsedVerb.tag).ToString();
                if (IsMatching(tag, "impf", "intr") && rgx.IsMatch(verb))
                {
                    return (new[] { rgx.Replace(verb, "") }).Backsert(items.Skip(1), 0);
                }
            }
            return items;

        }
    }
}
