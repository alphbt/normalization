using normalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalization
{
    public static class Difference
    {
        private static IEnumerable<VerbInfo> GetDifference<T>(IEnumerable<T> first, IEnumerable<T> second) where T: VerbInfo
        {
            return first.Except(second, new VerbsComparer());
        }
        public static IEnumerable<VerbWithFrequencyInfo> GetDifferenceOfCoSyCoCrossLexica(Dictionary<VerbWithFrequencyInfo, HashSet<VerbWithFrequencyInfo>> cosyco, 
            Dictionary<VerbInfo, HashSet<VerbInfo>> crossLexica)
        {
            var difference = GetDifference(cosyco.Keys, crossLexica.Keys);

            return cosyco.Keys.Join(difference, e => e.Verb, x => x.Verb, (e, x) => new VerbWithFrequencyInfo()
            {
                Verb = x.Verb,
                Prep = x.Prep,
                NounFrequency = e.NounFrequency,
                VerbFrequency = e.VerbFrequency,
                CombinationFrequency = e.CombinationFrequency
            }).OrderByDescending(e => e.CombinationFrequency);
        }

        public static IEnumerable<VerbInfo> GetDifferenceOfCrossLexicaCoSyCo(Dictionary<VerbInfo, HashSet<VerbInfo>> crossLexica,
            Dictionary<VerbWithFrequencyInfo, HashSet<VerbWithFrequencyInfo>> cosyco)
        {
            return GetDifference(crossLexica.Keys, cosyco.Keys);
        }
    }
}
