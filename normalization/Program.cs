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
using Normalization;

namespace NetCoreExample
{
    class Program
    {
        static void Main(string[] args)
        { 
            var perfPairsPath = @"..\..\..\..\..\..\PerfectVerbsPairs";
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            var verbsPairs = new InitialPerfectVerbs(perfPairsPath + @"\pairss.txt").GetVerbsPairs();


            var basePath = @"..\..\..\..\..\..\CrossLex\InitialFiles";
            
            var crossLexFiles = Directory.GetFiles(basePath);
            using var crossLex = new CrossLexica();

            foreach (var file in crossLexFiles)
            {
                var basePathCos = @"..\..\..\..\..\..\CoSyCo\";
                var fileName = Regex.Replace(Path.GetFileName(file), ".txt", "", RegexOptions.IgnoreCase);
                basePathCos += fileName + @"\" + fileName;
                var pth = @"..\..\..\..\..\..\CoSyCo\" + fileName;

                var cosyco = new CoSyCo();
                cosyco.Load(fileName);

                cosyco.WriteVerbsWithPreposotionToCsv(pth, "_ПРЕДЛОГ");
                cosyco.WriteVerbsWithoutPreposotionToCsv(pth, "_БЕЗ_ПРЕДЛОГА");
                cosyco.WriteVerbsCollectionToCsv(pth);

                var mixCo = cosyco.NormalizeVerbs(verbsPairs);
                mixCo.Keys.WriteToCsv(pth, fileName, "_НОРМ");

                crossLex.Load(file);
                //var currentPath = @"..\..\..\..\..\..\CrossLex\" + fileName;
                var allCombinations = crossLex.NormalizeVerbs(verbsPairs);
                

                var dif = Difference.GetDifferenceOfCoSyCoCrossLexica(mixCo, allCombinations);
                dif.WriteToCsv(pth, fileName, "_ВЫЧ");

                var ost = Difference.GetDifferenceOfCrossLexicaCoSyCo(allCombinations, mixCo);
                ost.WriteToCsv(pth, fileName, "_ОСТ");
            }

        }
    }
}
