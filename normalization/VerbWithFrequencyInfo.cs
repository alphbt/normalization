using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace normalization
{
    public class VerbWithFrequencyInfo: VerbInfo
    {
        public int NounFrequency { get; set; }
        public int VerbFrequency { get; set; }
        public int CombinationFrequency { get; set; }
    }
}
