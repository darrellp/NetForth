using System.Collections.Generic;
using System.Linq;

namespace NetForth
{
	class Vocabulary
    {
		#region Private variables
		private static readonly Stack<Vocabulary> _vocabularies = new Stack<Vocabulary>();
        private readonly Dictionary<string, Evaluable> _mapStoEvl;
		#endregion

		#region Public properties
		internal static Vocabulary CurrentVocabulary => _vocabularies.Peek();
        internal string Name { get; }
		#endregion

		#region Constructors
        public Vocabulary(Dictionary<string, Evaluable> mapStoEvl = null, string name = "Anonymous")
        {
            _mapStoEvl = mapStoEvl ?? new Dictionary<string, Evaluable>();
            Name = name;
        }

        internal static void Init()
        {
            for (int i = 0; i < _vocabularies.Count - 1; i++)
            {
                _vocabularies.Pop();
            }
            AddVocabulary(new Vocabulary(null, "User"));
        }
		#endregion

		#region Vocabulary operations
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
		#endregion

		#region Overrides
		public override string ToString()
        {
            return Name;
        }
		#endregion
	}
}
