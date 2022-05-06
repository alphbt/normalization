// See https://aka.ms/new-console-template for more information
using System;
using System.Text;
using System.Threading.Tasks;
using normalization;
using Python.Included;
using Python.Runtime;
using ServiceStack.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace NetCoreExample
{
    class Program
    {
        static void Main(string[] args)
        { 
            var perfPairsPath = @"..\..\..\..\..\..\PerfectVerbsPairs";
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            var verbsPairs = new InitialPerfectVerbs(perfPairsPath + @"\pairss.txt").GetVerbsPairs();

            var csvFile = CsvSerializer.SerializeToCsv(verbsPairs);
            var des = CsvSerializer.DeserializeFromString<List<List<string>>>(csvFile).Skip(1);

            var basePath = @"..\..\..\..\..\..\CrossLex\InitialFiles";
            
            var crossLexFiles = Directory.GetFiles(basePath);
            using var crossLex = new CrossLexica();

            foreach (var file in crossLexFiles)
            {
                var basePathCos = @"..\..\..\..\..\..\CoSyCo\";
                var fileName = Regex.Replace(Path.GetFileName(file), ".txt", "", RegexOptions.IgnoreCase);
                basePathCos += fileName + @"\" + fileName;

                var cosyco = new CoSyCo();
                cosyco.Load(fileName);
                var mixCo = cosyco.NormalizeVerbs(verbsPairs).Keys;

                //var pth = @"..\..\..\..\..\..\CoSyCo\" + fileName;
                //var mixCoSyCo = CsvSerializer.SerializeToCsv(mixCo);
                //File.WriteAllText(string.Format("{0}\\{1}{2}{3}", pth, fileName, "_НОРМ", ".csv"), mixCoSyCo, Encoding.UTF8);

                crossLex.Load(file);
                //var currentPath = @"..\..\..\..\..\..\CrossLex\" + fileName;
                var allCombinations = crossLex.NormalizeVerbs(verbsPairs).Keys;

                //var mixCrossLex = CsvSerializer.SerializeToCsv(allCombinations);
                //File.WriteAllText(string.Format("{0}\\{1}{2}{3}", currentPath, fileName, "_НОРМ", ".csv"), mixCrossLex, Encoding.UTF8);

                var dif = mixCo.Except(allCombinations, new VerbsComparer());
                //var mix = CsvSerializer.SerializeToCsv(dif);
                //File.WriteAllText(string.Format("{0}\\{1}{2}{3}", pth, fileName, "_ВЫЧ", ".csv"), mix, Encoding.UTF8);

                var crl = allCombinations.Except(mixCo, new VerbsComparer());
                //var cldif = CsvSerializer.SerializeToCsv(crl);
                //File.WriteAllText(string.Format("{0}\\{1}{2}{3}", pth, fileName, "_ОСТ", ".csv"), cldif, Encoding.UTF8);
            }

        }
    }
}
