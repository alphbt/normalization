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

            foreach (var property in items.GetType().GetProperties())
            {
                h = Combine(h, property != null ? property.GetHashCode() : 0);
            }

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
                var result = true;
                foreach(var property in x.GetType().GetProperties())
                {
                    var xValue = property.GetValue(x);
                    var yValue = property.GetValue(y);
                    if (!xValue.Equals(yValue)) 
                    { 
                        result = false; 
                        continue; 
                    }
                }
                return result;
            }

        }

        public int GetHashCode(VerbInfo obj) => MultiHash(obj);
    }
}
