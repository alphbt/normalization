using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace normalization
{
    public class SequentialStringComparer: IEqualityComparer<IEnumerable<string>>
    {
        private static int MultiHash(IEnumerable<object> items)
        {
            int h = 0;

            foreach (object item in items)
            {
                h = Combine(h, item != null ? item.GetHashCode() : 0);
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
        public bool Equals(IEnumerable<string>? x, IEnumerable<string>? y) => Enumerable.SequenceEqual(x!, y!);
        public int GetHashCode(IEnumerable<string> obj) => MultiHash(obj);
        
    }
}
