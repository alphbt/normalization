using static MoreLinq.Extensions.BacksertExtension;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConnectionToCoSyCo;
using System.Linq.Expressions;

namespace normalization
{
    public class CoSyCo
    {
        private string token;

        private IEnumerable<VerbWithFrequencyInfo> verbWithPreposition;

        private IEnumerable<VerbWithFrequencyInfo> verbWithoutPreposition;

        private IEnumerable<VerbWithFrequencyInfo> combinations;

        private static CosycoContext context = new CosycoContext();

        private IQueryable<dynamic> joinTable = context.noun_verb_comb_inf.Join(context.noun_verb_main_dict_inf,
                    comb => comb.id_main,
                    verb => verb.id_inf,
                    (comb, verb) => new { comb, verb })
                .Join(context.noun_verb_tail_dict_inf,
                    comb => comb.comb.id_tail,
                    noun => noun.id_inf,
                    (comb, noun) => new { comb, noun });

        public async Task Load(string token)
        {
            this.token = token;
            await SetVerbWithPreposition();
            await SetVerbWithoutPreposition();
            combinations = verbWithPreposition.Backsert(verbWithoutPreposition, 0);
        }
        private async Task<IEnumerable<VerbWithFrequencyInfo>> GetCombination(bool isEqual)
        {
            var joinTable = context.noun_verb_comb_inf.Join(context.noun_verb_main_dict_inf,
                    comb => comb.id_main,
                    verb => verb.id_inf,
                    (comb, verb) => new { comb, verb })
                .Join(context.noun_verb_tail_dict_inf,
                    comb => comb.comb.id_tail,
                    noun => noun.id_inf,
                    (comb, noun) => new { comb, noun });

            var resultTable = await joinTable.Where(e => e.noun.token.Equals(this.token) && 
                e.comb.comb.prep_word.Equals("") == isEqual)
                .GroupBy(e => new
                {
                    e.comb.comb.main_word,
                    e.comb.comb.prep_word,
                    e.comb.comb.tail_word
                })
                .Select(n => new
                {
                    Verb = n.Key.main_word,
                    Prep = n.Key.prep_word,
                    CombinationFrequency = n.Sum(c => c.comb.comb.freq),
                    VerbFrequency = n.Sum(c => c.comb.verb.freq),
                    NounFrequency = n.Sum(c => c.noun.freq)
                })
                .OrderByDescending(n => n.CombinationFrequency).ToListAsync();

            return resultTable.Select(e => new VerbWithFrequencyInfo()
            {
                Verb = e.Verb,
                Prep = e.Prep,
                NounFrequency = e.NounFrequency,
                VerbFrequency = e.VerbFrequency,
                CombinationFrequency = e.CombinationFrequency
            });
        }
        private async Task SetVerbWithPreposition()
        {
            verbWithPreposition = await GetCombination(false);
        }
        private async Task SetVerbWithoutPreposition()
        {
            verbWithoutPreposition = await GetCombination(true);
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
            combinations.WriteToCsv(path, this.token.ToUpper(), suffix);
        }
        public Dictionary<VerbWithFrequencyInfo, HashSet<VerbWithFrequencyInfo>> NormalizeVerbs(IDictionary<string, string> perfectVerbsDict)
        {
            return combinations.CompressVerb(perfectVerbsDict);
        }
    }
}
