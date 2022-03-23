// See https://aka.ms/new-console-template for more information
using System;
using System.Text;
using System.Threading.Tasks;
using normalization;
using Python.Included;
using Python.Runtime;
using ServiceStack.Text;

namespace NetCoreExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            /*Installer.InstallPath = Path.GetFullPath(".");

            // see what the installer is doing
            Installer.LogMessage += Console.WriteLine;

            await Installer.SetupPython();

            PythonEngine.Initialize();

            if (!Installer.IsModuleInstalled("pymorphy2"))
            {
                Installer.TryInstallPip();
                Installer.PipInstallModule("pymorphy2");
            }

            dynamic pm = Py.Import("pymorphy2");
            dynamic morph = pm.MorphAnalyzer();

            PythonEngine.Shutdown();*/

            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            InfPerfVerb infPerfVerb = new InfPerfVerb();
            infPerfVerb.FullDictionary(@"C:\Users\dasha\Desktop\norm\normalization\normalization\pairss.txt");

            var newList = infPerfVerb.PerfectVerbs.Select(l => new
            {
                perfForm = l.Key,
                infForm = l.Value
            }).ToList();
            
            var csvFile = CsvSerializer.SerializeToCsv(newList);
            File.WriteAllText("perfectVerbs.csv", csvFile, Encoding.UTF8);
            
            CrossLex crossLex = new CrossLex();
            var e = crossLex.GetNormalizedPhrases(@"C:\Users\dasha\Desktop\norm\normalization\normalization\CrossLex\InitialFiles\ДЕЛО.txt");

        }
    }
}
