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

            

            // install the embedded python distribution
            await Installer.SetupPython();

            // install pip3 for package installation
            ///Installer.TryInstallPip();

            // download and install Spacy from the internet
            ///Installer.PipInstallModule("spacy");

            // ok, now use pythonnet from that installation
            PythonEngine.Initialize();

            // call Python's sys.version to prove we are executing the right version
            dynamic sys = Py.Import("sys");
            Console.WriteLine("### Python version:\n\t" + sys.version);

            // call os.getcwd() to prove we are executing the locally installed embedded python distribution
            dynamic os = Py.Import("os");
            Console.WriteLine("### Current working directory:\n\t" + os.getcwd());
            Console.WriteLine("### PythonPath:\n\t" + PythonEngine.PythonPath);

            // call spacy
            dynamic spacy = Py.Import("spacy");
            Console.WriteLine("### Spacy version:\n\t" + spacy.__version__);


            dynamic np = Py.Import("numpy");
            Console.WriteLine(np.cos(np.pi * 2));

            if (!Installer.IsModuleInstalled("pymorphy2"))
            {
                Installer.TryInstallPip();
                Installer.PipInstallModule("pymorphy2");
            }

            dynamic pm = Py.Import("pymorphy2");
            dynamic morph = pm.MorphAnalyzer();
            Console.WriteLine(morph.parse("начать"));

            Console.WriteLine("\nDone. Press any key to exit.");

            Console.WriteLine("test");
            //Console.ReadKey();

            PythonEngine.Shutdown();*/

            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            InfPerfVerb infPerfVerb = new InfPerfVerb();
            infPerfVerb.FullDictionary(@"C:\Users\dasha\Desktop\norm\normalization\normalization\pairss.txt");

            foreach(KeyValuePair<string, HashSet<string>> entry in infPerfVerb.PerfectVerbs)
            {
                foreach(var val in entry.Value)
                {
                    Console.WriteLine("Key = {0}, Value = {1}", entry.Key, val);
                }
            }
        }
    }
}



//C: \Users\dasha\Desktop\pythonProject\venv