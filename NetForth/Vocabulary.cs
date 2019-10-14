using System.Collections.Generic;
using System.Linq;

namespace NetForth
{
	class Vocabulary
    {
        private static readonly Stack<Vocabulary> _vocabularies = new Stack<Vocabulary>();
        internal static Vocabulary CurrentVocabulary => _vocabularies.Peek();

        private readonly Dictionary<string, Evaluable> _mapStoEvl;
        internal string Name { get; }

        static Vocabulary()
        {
            RootPrims.AddRoot();
        }

        public Vocabulary(Dictionary<string, Evaluable> mapStoEvl = null, string name = "Anonymous")
        {
            _mapStoEvl = mapStoEvl ?? new Dictionary<string, Evaluable>();
            Name = name;
        }

        internal static void Init()
        {
            //_vocabularies.Clear();
            //RootPrims.AddRoot();
            for (int i = 0; i < _vocabularies.Count - 1; i++)
            {
                _vocabularies.Pop();
            }
            AddVocabulary(new Vocabulary(null, "User"));
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
            var lcWord = word.ToLower();
            _mapStoEvl[lcWord] = eval;
        }

        internal static void AddVocabulary(Vocabulary vocabulary)
        {
            _vocabularies.Push(vocabulary);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
