using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace normalization
{
    public class VerbsComparer : IEqualityComparer<VerbInfo>
    {
        private static int MultiHash(VerbInfo items)
        {
            int h = 0;

            h = Combine(h, items.Verb != null ? items.Verb.GetHashCode()  : 0);
            //h = Combine(h, items.Verb != null ? items.Prep.GetHashCode() : 0);
            
            return h;
        }
        private static int Combine(int x, int y)
        {
            unchecked
            {
                return (x << 5) + 3 + x ^ y;
            }
        }
        public bool Equals(VerbInfo? x, VerbInfo? y)
        {
            if (x == null && y == null) return false;
            else
            {
                return x.Verb == y.Verb; //&& x.Prep == y.Prep;
            }

        }

        public int GetHashCode(VerbInfo obj) => MultiHash(obj);
    }
}
