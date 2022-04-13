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
            var CoSyCoDeserialize = CsvSerializer.DeserializeFromString<IEnumerable<IEnumerable<string>>>(File.ReadAllText(fileName)).Skip(1);
            return CoSyCoDeserialize.Select(x => new VerbWithFrequencyInfo()
            {
                Verb = x.ElementAt(0).ToLower(),
                Prep = x.ElementAt(1).ToLower(),
                NounFrequency = Convert.ToInt32(x.ElementAt(2)),
                VerbFrequency = Convert.ToInt32(x.ElementAt(3)),
                CombinationFrequency = Convert.ToInt32(x.ElementAt(4))
            });
        }
    }
}
