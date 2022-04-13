using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace normalization
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> If<T>(this IEnumerable<T> source,Func<IEnumerable<T>,IEnumerable<T>> transformer, bool condition)
        {
            return condition ? transformer(source) : source;
        }
    }
}
