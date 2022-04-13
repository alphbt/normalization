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

namespace NetCoreExample
{
    class Program
    {
        static void Main(string[] args)
        { 
            var perfPairsPath = @"..\..\..\..\PerfectVerbsPairs";
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            var verbsPairs = new InitialPerfectVerbs(perfPairsPath + @"\pairss.txt").GetVerbsPairs();

            var csvFile = CsvSerializer.SerializeToCsv(verbsPairs);
            var des = CsvSerializer.DeserializeFromString<List<List<string>>>(csvFile).Skip(1);
            //File.WriteAllText(perfPairsPath + @"\perfectVerbs.csv", csvFile, Encoding.UTF8);

            var basePath = @"..\..\..\..\CrossLex\InitialFiles";
            var crossLexFiles = Directory.GetFiles(basePath);

        //    using var crossLex = new CrossLex();

        //    foreach (var file in crossLexFiles)
        //    {
        //        var fileName = Regex.Replace(Path.GetFileName(file), ".txt", "", RegexOptions.IgnoreCase);
        //        var mixedVerbs = crossLex.MixVerbs(file, verbsPairs).Keys;
        //        var mixed = mixedVerbs.Select(l => new
        //        {
        //            Verb = l.First(),
        //            Prep = string.Join(" ", l.Skip(1))
        //        });

        //        var currentPath = @"..\..\..\..\CrossLex" + @"\" + fileName;
        //        var mix = CsvSerializer.SerializeToCsv(mixed);
        //        File.WriteAllText(string.Format("{0}\\{1}{2}{3}", currentPath, fileName, "_скл", ".csv"), mix, Encoding.UTF8);


        //        ////var hasPredicates = crossLex.GetHasPredicates(file);
        //        ////var governedByVerb = crossLex.GetGovernedByVerb(file);
        //        ////var allCombinations = crossLex.GetNormalizedPhrases(file);

        //        //////var distincCom = Normalizator.RemoveIntransivity(allCombinations);

        //        ////var withoutPerf = Normalizator.RemovePerf(allCombinations, infPerfVerb.PerfectVerbs);
        //        ////var distincCom = Normalizator.RemoveIntransivity(withoutPerf);

        //        ////var currentPath = @"..\..\..\..\CrossLex" + @"\" + fileName;
        //        ////var dirInfo = Directory.CreateDirectory(currentPath);

        //        ////var csvHasPredicates = CsvSerializer.SerializeToCsv(hasPredicates);
        //        ////File.WriteAllText(string.Format("{0}\\{1}{2}{3}", currentPath, fileName,"_ИМЕЕТ_СКАЗУЕМОЕ", ".csv"), csvHasPredicates, Encoding.UTF8);

        //        ////var csvGovernedByVerb = CsvSerializer.SerializeToCsv(governedByVerb);
        //        ////File.WriteAllText(string.Format("{0}\\{1}{2}{3}", currentPath, fileName,"_УПРАВЛЯЕТСЯ_ГЛАГОЛОМ", ".csv"), csvGovernedByVerb, Encoding.UTF8);

        //        ////var csvAllCombinations = CsvSerializer.SerializeToCsv(allCombinations);
        //        ////File.WriteAllText(string.Format("{0}\\{1}{2}", currentPath, fileName, ".csv"), csvAllCombinations, Encoding.UTF8);

        //        ////var csvDistinctCom = CsvSerializer.SerializeToCsv(distincCom);
        //        ////File.WriteAllText(string.Format("{0}\\{1}{2}{3}", currentPath, fileName, "_БЕЗ_НЕВОЗВ", ".csv"), csvDistinctCom, Encoding.UTF8);

        //        ////var csvPerf = CsvSerializer.SerializeToCsv(withoutPerf);
        //        ////File.WriteAllText(string.Format("{0}\\{1}{2}{3}", currentPath, fileName, "_БЕЗ_СОВЕР", ".csv"), csvPerf, Encoding.UTF8);
        //    }

        //    //var allCombinations = crossLex

        //    PythonEngine.Shutdown();
        }
    }
}
