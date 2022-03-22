using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace normalization
{
    class ListComparer<T> : IEqualityComparer<List<List<T>>>
                           where T : IComparable<T>
    {
        public bool Equals(List<List<T>>? x, List<List<T>>? y)
        {
            if (x == y)
                return true;
            if ((x == null) || (y == null))
                return false;


            return Enumerable.SequenceEqual(x, y);
        }

        public int GetHashCode([DisallowNull] List<List<T>> obj)
        {
            unchecked
            {
                int hash = 19;
                foreach (var foo in obj)
                {
                    hash = hash * 31 + foo.GetHashCode();
                }
                return hash;
            }
        }
    }
}
