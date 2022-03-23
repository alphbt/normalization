using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nito.AsyncEx.Synchronous;
using Python.Included;
using Python.Runtime;
using static MoreLinq.Extensions.InsertExtension;
using static MoreLinq.Extensions.BacksertExtension;


namespace normalization
{
    public class CrossLex : IDisposable
    {
        static int MultiHash(IEnumerable<object> items)
        {
            int h = 0;

            foreach (object item in items)
            {
                h = Combine(h, item != null ? item.GetHashCode() : 0);
            }

            return h;
        }
        static int Combine(int x, int y)
        {
            unchecked
            {
                // This isn't a particularly strong way to combine hashes, but it's
                // cheap, respects ordering, and should work for the majority of cases.
                return (x << 5) + 3 + x ^ y;
            }
        }
        protected internal class SequentialStringComparer : IEqualityComparer<IEnumerable<string>>
        {
            public bool Equals(IEnumerable<string>? x, IEnumerable<string>? y) => Enumerable.SequenceEqual(x!, y!);
            public int GetHashCode(IEnumerable<string> obj) => MultiHash(obj);
        }

        static private List<string> tags = new List<string>() { "коллок", "локал", "книжн", "вульг", "неправ", "mb", "fig", "идиом", "и", "прямое", "не" };
        private bool disposedValue;

        static private dynamic importerPyMorphy2;
        static private dynamic morphAnalyzer;

        public CrossLex()
        {
            Installer.InstallPath = Path.GetFullPath(".");

            Installer.SetupPython().WaitAndUnwrapException();
            PythonEngine.Initialize();

            if (!Installer.IsModuleInstalled("pymorphy2"))
            {
                Installer.TryInstallPip();
                Installer.PipInstallModule("pymorphy2");
            }

            importerPyMorphy2 = Py.Import("pymorphy2");
            morphAnalyzer = importerPyMorphy2.MorphAnalyzer();
        }

        static IEnumerable<IEnumerable<string>> SplitString(IEnumerable<string> l)
        {
            return l.Select(x => x.Split(new char[] { ' ', '.', '(', ')', '/' }, StringSplitOptions.RemoveEmptyEntries))
                        .Where(x => x.Count() != 0);
        }

        static IEnumerable<string> ReadFromFile(string fileName, string crossLexSection)
        {
            return File.ReadLines(fileName, Encoding.UTF8)
                                    .SkipWhile(l => !l.Contains(crossLexSection))
                                    .Skip(2)
                                    .TakeWhile(l => l.Count() != 0);
        }

        static IEnumerable<IEnumerable<string>> RemoveTags(IEnumerable<IEnumerable<string>> l)
        {
            return l.Select(x => x.Select(y => y).Where(y => !tags.Contains(y)));
        }

        static IEnumerable<IEnumerable<string>> NormalizeVerb(IEnumerable<IEnumerable<string>> enumerable)
        {
            return enumerable.Select(l =>
            {
                var seqVerb = l.First();

                string normalForm = "";

                foreach (var i in morphAnalyzer.parse(seqVerb))
                {
                    string r = (i.tag).ToString();
                    if (r.Contains("INFN") || r.Contains("VERB"))
                    {
                        normalForm = (i.normal_form).ToString();
                        break;
                    }
                }

                return l.Skip(1).Insert(new[] { normalForm }, 0);
            });
        }

        static IEnumerable<IEnumerable<string>> RemoveRepeats(IEnumerable<IEnumerable<string>> enumerable) =>
                enumerable.Distinct(new SequentialStringComparer());

        public IEnumerable<Verb> GetHasPredicates(string fileName)
        {
            var hasPredicates = ReadFromFile(fileName, "Section: Has Predicates");
            var splitingHasPredicates = SplitString(hasPredicates);
            var withoutTagsHasPredicates = RemoveTags(splitingHasPredicates);
            var withoutMainNounHasPredicates = withoutTagsHasPredicates.Select(l => l.Select(x => x).TakeLast(l.Count() - 1).Take(1));
            var normalizedHasPredicates = NormalizeVerb(withoutMainNounHasPredicates);
            var uniqueHasPredicates = RemoveRepeats(normalizedHasPredicates);
            return uniqueHasPredicates.Select(e => new Verb()
                                             {
                                                 InfForm = e.First(),
                                                 Prep = string.Join(" ", e.Skip(1))
                                             })
                                             .ToList();
        }

        public IEnumerable<Verb> GetGovernedByVerb(string fileName)
        {
            var governedByVerb = ReadFromFile(fileName, "Section: Governed by Verbs");
            var splitingGovernedByVerb = SplitString(governedByVerb);
            var withoutTagsGovernedByVerb = RemoveTags(splitingGovernedByVerb);
            var withoutMainNounGoverenedByVerb = withoutTagsGovernedByVerb.Select(l => l.Select(x => x).Take(l.Count() - 1));
            var normalizedGovernedByVerb = NormalizeVerb(withoutMainNounGoverenedByVerb);
            var uniqueGovernedByVerb = RemoveRepeats(normalizedGovernedByVerb);
            return uniqueGovernedByVerb.Select(e => new Verb()
                                               {
                                                   InfForm = e.First(),
                                                   Prep = string.Join(" ", e.Skip(1))
                                               })
                                               .ToList();
        }

        public IEnumerable<Verb> GetNormalizedPhrases(string fileName)
        {
            var hasPredicates = ReadFromFile(fileName, "Section: Has Predicates");
            var governedByVerb = ReadFromFile(fileName, "Section: Governed by Verbs");

            var splitingHasPredicates = SplitString(hasPredicates);
            var splitingGovernedByVerb = SplitString(governedByVerb);

            var withoutTagsHasPredicates = RemoveTags(splitingHasPredicates);
            var withoutTagsGovernedByVerb = RemoveTags(splitingGovernedByVerb);

            var withoutMainNounHasPredicates = withoutTagsHasPredicates.Select(l => l.Select(x => x).TakeLast(l.Count() - 1)).Take(1);
            var withoutMainNounGoverenedByVerb = withoutTagsGovernedByVerb.Select(l => l.Select(x => x).Take(l.Count() - 1));

            var normalizedHasPredicates = NormalizeVerb(withoutMainNounHasPredicates);
            var normalizedGovernedByVerb = NormalizeVerb(withoutMainNounGoverenedByVerb);

            var uniqueHasPredicates = RemoveRepeats(normalizedHasPredicates);
            var uniqueGovernedByVerb = RemoveRepeats(normalizedGovernedByVerb);

            return uniqueHasPredicates.Backsert(uniqueGovernedByVerb, 0).Select(e =>
                                                                                new Verb()
                                                                                {
                                                                                    InfForm = e.First(),
                                                                                    Prep = string.Join(" ", e.Skip(1))
                                                                                })
                                                                                .ToList();
        }


        #region Disposing

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                    PythonEngine.Shutdown();
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей

                //PythonEngine.Shutdown();

                disposedValue = true;
            }
        }
         // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        ~CrossLex()
        {
            
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: false);
        }
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
