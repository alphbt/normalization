using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace normalization
{
    public class VerbWithFrequencyInfo : VerbInfo
    {
        public double NounFrequency { get; set; }
        public double VerbFrequency { get; set; }
        public double CombinationFrequency { get; set; }

        public double LogDice() => Math.Log2(2 * this.CombinationFrequency / (this.NounFrequency + this.VerbFrequency)) + 14;
        public double MinSen() => Math.Min(this.CombinationFrequency / this.NounFrequency , this.CombinationFrequency / this.VerbFrequency);
    }
}
