using System.Collections.Generic;
using System.Linq;

namespace NetForth
{
	class Vocabulary
    {
        private static readonly List<Vocabulary> _vocabularies = new List<Vocabulary>();
        internal static Vocabulary CurrentVocabulary => _vocabularies[_vocabularies.Count - 1];

		private readonly Dictionary<string, Evaluable> _mapStoEvl;

        static Vocabulary()
        {
            RootPrims.AddRoot();
        }
        public Vocabulary(Dictionary<string, Evaluable> mapStoEvl = null)
        {
			_mapStoEvl = mapStoEvl ?? new Dictionary<string, Evaluable>();
        }

        internal static Evaluable Lookup(string word)
        {
            var lcWord = word.ToLower();
            return (
                from vocabulary in _vocabularies
                where vocabulary._mapStoEvl.ContainsKey(lcWord)
                select vocabulary._mapStoEvl[lcWord]).FirstOrDefault();
        }

        public void AddDefinition(string word, Evaluable eval)
        {
            _mapStoEvl[word] = eval;
        }

        internal static void AddVocabulary(Vocabulary vocabulary)
        {
            _vocabularies.Add(vocabulary);
        }
    }
}
