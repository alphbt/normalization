// See https://aka.ms/new-console-template for more information
using System;
using System.Text;
using System.Threading.Tasks;
using Python.Included;
using ServiceStack.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using Normalization;
using Python.Runtime;
using Nito.AsyncEx.Synchronous;

namespace NetCoreExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Installer.InstallPath = Path.GetFullPath(".");
            Installer.SetupPython().WaitWithoutException();
            PythonEngine.Initialize();
            var noname = MorphAnalyzer.Instance;

            var perfPairsPath = @"..\..\..\..\..\..\PerfectVerbsPairs\perfectVerbs.csv";
            //System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            //var verbPairs = InitialPerfectVerbs.GetVerbsPairs(@"C:\Users\dasha\Desktop\Normalization\PerfectVerbsPairs\pairss.txt");
            var verbPairs = InitialPerfectVerbs.ReadVerbPairsFromCsv(perfPairsPath);

            var basePath = @"..\..\..\..\..\..\CrossLex\InitialFiles";

            var crossLexFiles = Directory.GetFiles(basePath);
            var crossLex = new CrossLexica();
            var cosyco = new CoSyCo();

            foreach (var file in crossLexFiles)
            {
                var basePathCos = @"..\..\..\..\..\..\CoSyCo\";
                var fileName = Regex.Replace(Path.GetFileName(file), ".txt", "", RegexOptions.IgnoreCase);
                basePathCos += fileName + @"\" + fileName;
                var pth = @"..\..\..\..\..\..\CoSyCo\" + fileName;

                await cosyco.Load(fileName);

                //cosyco.WriteVerbsWithPreposotionToCsv(pth, "_ПРЕДЛОГ");
                //cosyco.WriteVerbsWithoutPreposotionToCsv(pth, "_БЕЗ_ПРЕДЛОГА");
                // cosyco.WriteVerbsCollectionToCsv(pth);

                var mixCo = cosyco.NormalizeVerbs(verbPairs).CompressVervsWithoutPreposition().Keys;
                mixCo.WriteToCsv(pth, fileName, "_НОРМ");

                crossLex.Load(file);
                var currentPath = @"..\..\..\..\..\..\CrossLex\" + fileName;
                var allCombinations = crossLex.NormalizeVerbs(verbPairs).CompressVervsWithoutPreposition().Keys;

                allCombinations.WriteToCsv(currentPath, fileName, "_НОРМ");

                var dif = Difference.GetDifferenceOfCoSyCoCrossLexica(mixCo, allCombinations);
                dif.WriteToCsv(pth, fileName, "_ВЫЧ");

                var difp = dif.Where(x => x.LogDice() > 0);
                difp.WriteToCsv(pth, fileName, "_ПОРОГ_" + difp.Count().ToString());

                var ost = Difference.GetDifferenceOfCrossLexicaCoSyCo(allCombinations, mixCo);
                ost.WriteToCsv(pth, fileName, "_ОСТ");
            }

            MorphAnalyzer.Instance.Dispose();
        }
    }
}
