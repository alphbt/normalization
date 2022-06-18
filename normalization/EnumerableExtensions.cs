using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalization
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> If<T>(this IEnumerable<T> source,Func<IEnumerable<T>,IEnumerable<T>> transformer, bool condition)
        {
            return condition ? transformer(source) : source;
        }

        public static void WriteToCsv<T>(this IEnumerable<T> source, string path,string name, string suffix = "")
        {
            var csvFile = CsvSerializer.SerializeToCsv(source);
            File.WriteAllText(string.Format("{0}\\{1}{2}{3}", path, name, suffix, ".csv"), csvFile, Encoding.UTF8);
        }
    }
}
