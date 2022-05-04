using static MoreLinq.Extensions.BacksertExtension;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace normalization
{
    public class CoSyCo
    {
        private string fileName;

        public CoSyCo(string fileName)
        {
            this.fileName = fileName;
        }

        public IEnumerable<VerbWithFrequencyInfo> GetCoSyCoCollection()
        {
            var basePath = fileName + "_БЕЗ_ПРЕДЛ.csv";
            var without = fileName + ".csv";
            var CoSyCoDeserialize = CsvSerializer.DeserializeFromString<IEnumerable<IEnumerable<string>>>(File.ReadAllText(without)).Skip(1);
            var csc = CsvSerializer.DeserializeFromString<IEnumerable<IEnumerable<string>>>(File.ReadAllText(basePath)).Skip(1);
            var res = CoSyCoDeserialize.Backsert(csc, 0);
            
            return res.Select(x => new VerbWithFrequencyInfo()
            {
                Verb = x.ElementAt(0).ToLower(),
                Prep = x.ElementAt(1) == null ? "" : x.ElementAt(1).ToLower(),
                NounFrequency = Convert.ToDouble(x.ElementAt(2)),
                VerbFrequency = Convert.ToDouble(x.ElementAt(3)),
                CombinationFrequency = Convert.ToDouble(x.ElementAt(4))
            });
        }

        public IDictionary<VerbWithFrequencyInfo, HashSet<VerbWithFrequencyInfo>> GetMixed(IDictionary<string, string> perfectVerbsDict)
        {
            var initialVerbsForm = GetCoSyCoCollection();

            return initialVerbsForm.CompressVerb(perfectVerbsDict);
        }
    }
}
