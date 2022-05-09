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
        private static dynamic? GetAnalyzerResult<T>(this T items) where T : VerbInfo
        {
            var verb = items.Verb;
            return MorphAnalyzer.Instance.Analyzer.parse(verb);
        }

        private static bool IsMatching(string tags, params string[] tagNames)
        {
            return tagNames.All(x => tags.Contains(x));
        }

        public static T PerfectTransitiveRule<T>(this T items, IDictionary<string, string> perfectVerbsDict) where T : VerbInfo, new()
        {
            var analyzerResult = GetAnalyzerResult(items);
            var verb = items.Verb;

            foreach (var parsedVerb in analyzerResult!)
            {
                var tag = (parsedVerb.tag).ToString();
                if (IsMatching(tag, "perf", "tran") && perfectVerbsDict.ContainsKey(verb))
                {
                    var res = new T();
                    var prop = res.GetType().GetProperties();
                    foreach (var property in prop)
                    {
                        property.SetValue(res, items.GetType().GetProperty(property.Name).GetValue(items));
                    }
                    res.Verb = perfectVerbsDict[verb];

                    return res;
                }
            }
            return items;
        }

        public static T PerfectIntransiveWithoutEndRule<T>(this T items, IDictionary<string, string> perfVerbsDict) where T : VerbInfo, new()
        {
            var analyzerResult = GetAnalyzerResult(items);
            var verb = items.Verb;
            var rgx = new Regex(@"\w*(сь|ся)$");

            foreach (var parsedVerb in analyzerResult!)
            {
                var tag = (parsedVerb.tag).ToString();
                if (IsMatching(tag, "perf", "intr") && !rgx.IsMatch(verb) && perfVerbsDict.ContainsKey(verb))
                {
                    var res = new T();
                    var prop = res.GetType().GetProperties();
                    foreach (var property in prop)
                    {
                        property.SetValue(res, items.GetType().GetProperty(property.Name).GetValue(items));
                    }
                    res.Verb = perfVerbsDict[verb];

                    return res;
                }
            }
            return items;
        }

        public static T PerfectIntransiveRule<T>(this T items, IDictionary<string, string> perfVerbsDict) where T : VerbInfo, new()
        {
            var analyzerResult = GetAnalyzerResult(items);
            var verb = items.Verb;
            var rgx = new Regex(@"(сь|ся)$");

            foreach (var parsedVerb in analyzerResult!)
            {
                var tag = (parsedVerb.tag).ToString();
                if (IsMatching(tag, "perf", "intr") && rgx.IsMatch(verb))
                {
                    verb = rgx.Replace(verb, "");
                    var res = new T();
                    var prop = res.GetType().GetProperties();
                    foreach (var property in prop)
                    {
                        property.SetValue(res, items.GetType().GetProperty(property.Name).GetValue(items));
                    }
                    res.Verb = verb;

                    if (perfVerbsDict.ContainsKey(res.Verb))
                        res.Verb = perfVerbsDict[res.Verb];

                    return res;
                }
            }
            return items;
        }

        public static T ImperfectIntransitiveRule<T>(this T items) where T : VerbInfo, new()
        {
            var analyzerResult = GetAnalyzerResult(items);
            var verb = items.Verb;
            var rgx = new Regex(@"(сь|ся)$");

            foreach (var parsedVerb in analyzerResult!)
            {
                var tag = (parsedVerb.tag).ToString();
                if (IsMatching(tag, "impf", "intr") && rgx.IsMatch(verb))
                {
                    verb = rgx.Replace(verb, "");
                    var res = new T();
                    var prop = res.GetType().GetProperties();
                    foreach (var property in prop)
                    {
                        property.SetValue(res, items.GetType().GetProperty(property.Name).GetValue(items));
                    }
                    res.Verb = verb;

                    return res;
                }
            }
            return items;

        }
    }
}
