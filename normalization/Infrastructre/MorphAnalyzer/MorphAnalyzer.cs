using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Included;
using Nito.AsyncEx.Synchronous;
using Python.Runtime;

namespace Normalization
{
    public sealed class MorphAnalyzer : IDisposable
    {
        private static dynamic? instance;
        private static dynamic? analyzer;
        private bool disposedValue;
        private IntPtr threadState;

        private MorphAnalyzer()
        {
            //Installer.InstallPath = Path.GetFullPath(".");
            //Installer.SetupPython().WaitWithoutException();
            //PythonEngine.Initialize();

            
            threadState = PythonEngine.BeginAllowThreads();
            using (Py.GIL())
            {
                if (!Installer.IsModuleInstalled("pymorphy2"))
                {
                    Installer.TryInstallPip();
                    Installer.PipInstallModule("pymorphy2");
                }

                dynamic importerPyMorphy2 = Py.Import("pymorphy2");
                analyzer = importerPyMorphy2.MorphAnalyzer();


                disposedValue = false;
            }
        }

        public static MorphAnalyzer Instance
        {
            get
            {
                if (instance == null)
                    instance = new MorphAnalyzer();

                return instance;
            }
        }

        public dynamic Analyzer
        {
            get
            {
                return analyzer;
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //instance = null;
                    //analyzer = null;
                }
                
                disposedValue = true;

                PythonEngine.EndAllowThreads(threadState);
                PythonEngine.Shutdown();

                /*
                Runtime.TryCollectingGarbage(1);

                Py.GIL().Dispose();

                PythonEngine.Interrupt(PythonEngine.GetPythonThreadID());
                PythonEngine.Interrupt((ulong)Runtime.MainManagedThreadId);
                PythonEngine.Shutdown();
                */
            }
        }

         ~MorphAnalyzer()
         {
             Dispose(disposing: false);
         }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
