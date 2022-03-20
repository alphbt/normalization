// See https://aka.ms/new-console-template for more information
using System;
using System.Threading.Tasks;
using normalization;
using Python.Included;
using Python.Runtime;


/*
string pathToVirtualEnv = $@"C:\Users\dasha\Desktop\Python\venv";

Environment.SetEnvironmentVariable("PATH", pathToVirtualEnv, EnvironmentVariableTarget.Process);
Environment.SetEnvironmentVariable("PYTHONHOME", pathToVirtualEnv, EnvironmentVariableTarget.Process);
Environment.SetEnvironmentVariable("PYTHONPATH", $"{pathToVirtualEnv}\\Lib\\site-packages;{pathToVirtualEnv}\\Lib", EnvironmentVariableTarget.Process);


//PythonEngine.PythonHome = pathToVirtualEnv;
//PythonEngine.PythonPath = Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process);

using (Py.GIL())
{
    dynamic np = Py.Import("numpy");
    Console.WriteLine(np.cos(np.pi * 2));

    Console.ReadKey();
}
*/

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

            foreach(KeyValuePair<string, string> entry in infPerfVerb.PerfectVerbs)
            {
               
                    Console.WriteLine("Key = {0}, Value = {1}", entry.Key, entry.Value );
                
            }
        }
    }
}



//C: \Users\dasha\Desktop\pythonProject\venv