using static MoreLinq.Extensions.BacksertExtension;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConnectionToCoSyCo;

namespace normalization
{
    public class CoSyCo
    {
        private string token;

        private CosycoContext context = new CosycoContext();

        private List<VerbWithFrequencyInfo> verbWithPreposition = new List<VerbWithFrequencyInfo>();
        private List<VerbWithFrequencyInfo> verbWithoutPreposition = new List<VerbWithFrequencyInfo>();
        public void Load(string token)
        {
            this.token = token;
            GetVerbWithPreposition();
            GetVerbWithoutPreposition();
        }
        private void GetVerbWithPreposition()
        {
            verbWithPreposition.Clear();

            var joinTable = context.noun_verb_comb_inf.Join(context.noun_verb_main_dict_inf,
                    verbComb => verbComb.id_main,
                    verb => verb.id_inf,
                    (verbComb, verb) => new { verbComb, verb })
                .Join(context.noun_verb_tail_dict_inf,
                    nounComb => nounComb.verbComb.id_tail,
                    noun => noun.id_inf,
                    (nounComb, noun) => new { nounComb, noun });

            var verbsWithPrepositionTable = joinTable.Where(e => e.noun.token.Equals(this.token) &&
                    !e.nounComb.verbComb.prep_word.Equals(""))
                .GroupBy(e => new {
                    e.nounComb.verbComb.main_word,
                    e.nounComb.verbComb.prep_word,
                    e.nounComb.verbComb.tail_word
                })
                .Select(n => new {
                    verb = n.Key.main_word,
                    prep = n.Key.prep_word,
                    freqComb = n.Sum(c => c.nounComb.verbComb.freq),
                    freqVerb = n.Sum(c => c.nounComb.verb.freq),
                    freqNoun = n.Sum(c => c.noun.freq),
                })
                .OrderByDescending(n => n.freqComb)
                //.Take(1000)
                .ToList();

            verbWithPreposition =  verbsWithPrepositionTable.Select(e => new VerbWithFrequencyInfo()
            {
                Verb = e.verb.ToLower(),
                Prep = e.prep.ToLower(),
                NounFrequency = e.freqNoun,
                VerbFrequency = e.freqVerb,
                CombinationFrequency = e.freqComb
            }).ToList();
        }
        private void GetVerbWithoutPreposition()
        {
            verbWithoutPreposition.Clear();

            var joinTable = context.noun_verb_comb_inf.Join(context.noun_verb_main_dict_inf,
                    verbComb => verbComb.id_main,
                    verb => verb.id_inf,
                    (verbComb, verb) => new { verbComb, verb })
                .Join(context.noun_verb_tail_dict_inf,
                    nounComb => nounComb.verbComb.id_tail,
                    noun => noun.id_inf,
                    (nounComb, noun) => new { nounComb, noun });

            var verbsWithoutPrepositionTable = joinTable.Where(e => e.noun.token.Equals(this.token) &&
                    e.nounComb.verbComb.prep_word.Equals(""))
                .GroupBy(e => new {
                    e.nounComb.verbComb.main_word,
                    e.nounComb.verbComb.prep_word,
                    e.nounComb.verbComb.tail_word
                })
                .Select(n => new {
                    verb = n.Key.main_word,
                    prep = n.Key.prep_word,
                    freqComb = n.Sum(c => c.nounComb.verbComb.freq),
                    freqVerb = n.Sum(c => c.nounComb.verb.freq),
                    freqNoun = n.Sum(c => c.noun.freq),
                })
                .OrderByDescending(n => n.freqComb)
                //.Take(1000)
                .ToList();

            verbWithoutPreposition = verbsWithoutPrepositionTable.Select(e => new VerbWithFrequencyInfo()
            {
                Verb = e.verb.ToLower(),
                Prep = e.prep.ToLower(),
                NounFrequency = e.freqNoun,
                VerbFrequency = e.freqVerb,
                CombinationFrequency = e.freqComb
            }).ToList();
        }
        private IEnumerable<VerbWithFrequencyInfo> GetCoSyCoCollection()
        {
            return verbWithPreposition.Backsert(verbWithoutPreposition, 0);
        }
        public void WriteVerbsWithPreposotionToCsv(string path, string suffix ="")
        {
            verbWithPreposition.WriteToCsv(path, this.token.ToUpper(), suffix);
        }
        public void WriteVerbsWithoutPreposotionToCsv(string path, string suffix = "")
        {
            verbWithoutPreposition.WriteToCsv(path, this.token.ToUpper(), suffix);
        }
        public void WriteVerbsCollectionToCsv(string path, string suffix = "")
        {
            GetCoSyCoCollection().WriteToCsv(path, this.token.ToUpper(), suffix);
        }
        public Dictionary<VerbWithFrequencyInfo, HashSet<VerbWithFrequencyInfo>> NormalizeVerbs(IDictionary<string, string> perfectVerbsDict)
        {
            var initialVerbsForm = GetCoSyCoCollection();

            return initialVerbsForm.CompressVerb(perfectVerbsDict);
        }
    }
}
