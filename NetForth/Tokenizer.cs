using System.IO;

namespace NetForth
{
    internal class Tokenizer
    {
		#region Private variables
        private string _currentLine;
        private int _ich;
		#endregion

		#region Public properties
		internal readonly TextReader Reader;
		#endregion

		#region Constructor
		internal Tokenizer(TextReader reader)
        {
            Reader = reader;
            // ReSharper disable once PossibleNullReferenceException
            _currentLine = reader.ReadLine().Trim();
        }
		#endregion

		#region Tokenization
		internal string NextToken(bool allowEof = false, bool returnEol = false)
        {
            if (_ich == _currentLine.Length)
            {
                _currentLine = Reader.ReadLine();
                _ich = 0;
                if (returnEol)
                {
                    return "\n";
                }

                if (_currentLine == null)
                {
                    if (allowEof)
                    {
                        return null;
                    }
                    throw new NfException("Unexpected EOF");
                }
            }

            while (char.IsWhiteSpace(_currentLine[_ich]))
            {
                ++_ich;
            }

            var ichStart = _ich;

            while (_ich < _currentLine.Length && !char.IsWhiteSpace(_currentLine[_ich]))
            {
                _ich++;
            }

            return _currentLine.Substring(ichStart, _ich - ichStart);
        }
		#endregion
	}
}
