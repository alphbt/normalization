using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Included;
using Python.Runtime;
using Nito.AsyncEx.Synchronous;

namespace normalization
{
    public sealed class MorphAnalyzer : IDisposable
    {
        private static dynamic? instance;
        private static dynamic? analyzer;
        private bool disposedValue;

        private MorphAnalyzer()
        {
            Installer.InstallPath = Path.GetFullPath(".");

            Installer.SetupPython().WaitAndUnwrapException();
            PythonEngine.Initialize();

            if (!Installer.IsModuleInstalled("pymorphy2"))
            {
                Installer.TryInstallPip();
                Installer.PipInstallModule("pymorphy2");
            }

            dynamic importerPyMorphy2 = Py.Import("pymorphy2");
            analyzer = importerPyMorphy2.MorphAnalyzer();
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
                    instance = null;
                    analyzer = null;
                }

                PythonEngine.Shutdown();
                disposedValue = true;
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
