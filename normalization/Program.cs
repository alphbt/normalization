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
            //File.WriteAllText(perfPairsPath + @"\perfectVerbs.csv", csvFile, Encoding.UTF8);

            var basePath = @"..\..\..\..\..\..\CrossLex\InitialFiles";
            
            var crossLexFiles = Directory.GetFiles(basePath);

            foreach (var file in crossLexFiles)
            {
                var basePathCos = @"..\..\..\..\..\..\CoSyCo\";
                var fileName = Regex.Replace(Path.GetFileName(file), ".txt", "", RegexOptions.IgnoreCase);
                basePathCos += fileName + @"\" + fileName;

                var cosyco = new CoSyCo(basePathCos);
                var mixCo = cosyco.GetMixed(verbsPairs).Keys;

                var pth = @"..\..\..\..\..\..\CoSyCo\" + fileName;
                var mixCoSyCo = CsvSerializer.SerializeToCsv(mixCo);
                File.WriteAllText(string.Format("{0}\\{1}{2}{3}", pth, fileName, "_скл", ".csv"), mixCoSyCo, Encoding.UTF8);

                var crossLex = new CrossLex(file);
                var currentPath = @"..\..\..\..\..\..\CrossLex\" + fileName;
                var allCombinations = crossLex.MixVerbs(verbsPairs).Keys;

                var mixCrossLex = CsvSerializer.SerializeToCsv(allCombinations);
                File.WriteAllText(string.Format("{0}\\{1}{2}{3}", currentPath, fileName, "_скл", ".csv"), mixCrossLex, Encoding.UTF8);

                var dif = mixCo.Except(allCombinations, new VerbsComparer());
                var crl = allCombinations.Except(mixCo, new VerbsComparer());

                
                var mix = CsvSerializer.SerializeToCsv(dif);
                File.WriteAllText(string.Format("{0}\\{1}{2}{3}", pth, fileName, "_рез", ".csv"), mix, Encoding.UTF8);

                
                var cldif = CsvSerializer.SerializeToCsv(crl);
                File.WriteAllText(string.Format("{0}\\{1}{2}{3}", pth, fileName, "_ост", ".csv"), cldif, Encoding.UTF8);

                //var currentPath = @"..\..\..\..\CrossLex\" + fileName;
                //var mix = CsvSerializer.SerializeToCsv(mixedVerbs);
                //File.WriteAllText(string.Format("{0}\\{1}{2}{3}", currentPath, fileName, "_скл", ".csv"), mix, Encoding.UTF8);


                ////var hasPredicates = crossLex.GetHasPredicates(file);
                ////var governedByVerb = crossLex.GetGovernedByVerb(file);

                //////var distincCom = Normalizator.RemoveIntransivity(allCombinations);

                ////var withoutPerf = Normalizator.RemovePerf(allCombinations, infPerfVerb.PerfectVerbs);
                ////var distincCom = Normalizator.RemoveIntransivity(withoutPerf);

                ////var currentPath = @"..\..\..\..\CrossLex" + @"\" + fileName;
                ////var dirInfo = Directory.CreateDirectory(currentPath);

                ////var csvHasPredicates = CsvSerializer.SerializeToCsv(hasPredicates);
                ////File.WriteAllText(string.Format("{0}\\{1}{2}{3}", currentPath, fileName,"_ИМЕЕТ_СКАЗУЕМОЕ", ".csv"), csvHasPredicates, Encoding.UTF8);

                ////var csvGovernedByVerb = CsvSerializer.SerializeToCsv(governedByVerb);
                ////File.WriteAllText(string.Format("{0}\\{1}{2}{3}", currentPath, fileName,"_УПРАВЛЯЕТСЯ_ГЛАГОЛОМ", ".csv"), csvGovernedByVerb, Encoding.UTF8);

                ////var csvAllCombinations = CsvSerializer.SerializeToCsv(allCombinations);
                ////File.WriteAllText(string.Format("{0}\\{1}{2}", currentPath, fileName, ".csv"), csvAllCombinations, Encoding.UTF8);

                ////var csvDistinctCom = CsvSerializer.SerializeToCsv(distincCom);
                ////File.WriteAllText(string.Format("{0}\\{1}{2}{3}", currentPath, fileName, "_БЕЗ_НЕВОЗВ", ".csv"), csvDistinctCom, Encoding.UTF8);

                ////var csvPerf = CsvSerializer.SerializeToCsv(withoutPerf);
                ////File.WriteAllText(string.Format("{0}\\{1}{2}{3}", currentPath, fileName, "_БЕЗ_СОВЕР", ".csv"), csvPerf, Encoding.UTF8);
            }

            PythonEngine.Shutdown();
        }
    }
}
