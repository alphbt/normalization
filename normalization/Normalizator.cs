using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace normalization
{
    public static class Normalizator
    {
        public static Dictionary<T, HashSet<T>> CompressVerb<T>(this IEnumerable<T> verbCollection, IDictionary<string, string> perfectVerbsDict)
            where T : VerbInfo, new()
        {
            var normalizedForms = new Dictionary<T, HashSet<T>>(new VerbsComparer());
            foreach (var verb in verbCollection)
            {
                var normalizedVerb = verb.ImperfectIntransitiveRule()
                    .PerfectIntransiveRule(perfectVerbsDict).PerfectIntransiveWithoutEndRule(perfectVerbsDict)
                    .PerfectTransitiveRule(perfectVerbsDict);

                if (!normalizedForms.ContainsKey(normalizedVerb))
                {
                    normalizedForms.Add(normalizedVerb, new HashSet<T>(new VerbsComparer()));
                }
                normalizedForms[normalizedVerb].Add(verb);
            }

            if(verbCollection.First().GetType().Equals(typeof(VerbWithFrequencyInfo)))
            {
                RecalculateMeasures(normalizedForms as Dictionary<VerbWithFrequencyInfo, HashSet<VerbWithFrequencyInfo>>);
            }

            return normalizedForms;
        }

        public static void RecalculateMeasures(Dictionary<VerbWithFrequencyInfo, HashSet<VerbWithFrequencyInfo>> normalizedForms)
        {
            var resultVerbs = new List<VerbWithFrequencyInfo>(normalizedForms.Keys);

            foreach (var verb in resultVerbs)
            {
                var values = normalizedForms[verb];
                var weights = values.Sum(e => e.MinSen());

                var weightedAveragesForVerb = Math.Round(values.Sum(e => e.MinSen() * e.VerbFrequency) / weights, 0);
                var weightedAveragesForComb = Math.Round(values.Sum(e => e.MinSen() * e.CombinationFrequency) / weights, 0);

                var normalizedVerb = new VerbWithFrequencyInfo
                {
                    Verb = verb.Verb,
                    Prep = verb.Prep,
                    NounFrequency = verb.NounFrequency,
                    VerbFrequency = weightedAveragesForVerb,
                    CombinationFrequency = weightedAveragesForComb
                };

                normalizedForms.Remove(verb);
                normalizedForms.Add(normalizedVerb, values);
            }
        }

    }
}
