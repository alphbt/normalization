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
        public static IEnumerable<VerbWithFrequencyInfo> GetDifferenceOfCoSyCoCrossLexica(IEnumerable<VerbWithFrequencyInfo> cosyco, 
            IEnumerable<VerbInfo> crossLexica)
        {
            var difference = GetDifference(cosyco, crossLexica);

            return cosyco.Join(difference, e => e.Verb, x => x.Verb, (e, x) => new VerbWithFrequencyInfo()
            {
                Verb = x.Verb,
                Prep = x.Prep,
                NounFrequency = e.NounFrequency,
                VerbFrequency = e.VerbFrequency,
                CombinationFrequency = e.CombinationFrequency
            })
            .OrderByDescending(e => e.CombinationFrequency);
        }

        public static IEnumerable<VerbInfo> GetDifferenceOfCrossLexicaCoSyCo(IEnumerable<VerbInfo> crossLexica,
            IEnumerable<VerbWithFrequencyInfo> cosyco)
        {
            return GetDifference(crossLexica, cosyco);
        }
    }
}
