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
            InfPerfVerb infPerfVerb = new InfPerfVerb();
            infPerfVerb.FullDictionary(perfPairsPath + @"\pairss.txt");

            var newList = infPerfVerb.PerfectVerbs.Select(l => new
            {
                perfForm = l.Key,
                infForm = l.Value
            }).ToList();
            
            var csvFile = CsvSerializer.SerializeToCsv(newList);
            File.WriteAllText(perfPairsPath + @"\perfectVerbs.csv", csvFile, Encoding.UTF8);

            var basePath = @"..\..\..\..\CrossLex\InitialFiles";
            var crossLexFiles = Directory.GetFiles(basePath);
            CrossLex crossLex = new CrossLex();
            foreach(var file in crossLexFiles)
            {
                var fileName = Regex.Replace(Path.GetFileName(file), ".txt", "", RegexOptions.IgnoreCase);
                var hasPredicates = crossLex.GetHasPredicates(file);
                var governedByVerb = crossLex.GetGovernedByVerb(file);
                var allCombinations = crossLex.GetNormalizedPhrases(file);

                var currentPath = @"..\..\..\..\CrossLex" + @"\" + fileName;
                var dirInfo = Directory.CreateDirectory(currentPath);

                var csvHasPredicates = CsvSerializer.SerializeToCsv(hasPredicates);
                File.WriteAllText(string.Format("{0}\\{1}{2}{3}", currentPath, fileName,"_ИМЕЕТ_СКАЗУЕМОЕ", ".csv"), csvHasPredicates, Encoding.UTF8);

                var csvGovernedByVerb = CsvSerializer.SerializeToCsv(governedByVerb);
                File.WriteAllText(string.Format("{0}\\{1}{2}{3}", currentPath, fileName,"_УПРАВЛЯЕТСЯ_ГЛАГОЛОМ", ".csv"), csvGovernedByVerb, Encoding.UTF8);

                var csvAllCombinations = CsvSerializer.SerializeToCsv(allCombinations);
                File.WriteAllText(string.Format("{0}\\{1}{2}", currentPath, fileName, ".csv"), csvAllCombinations, Encoding.UTF8);
            }
            PythonEngine.Shutdown();
        }
    }
}
