using System.IO;

namespace NetForth
{
    internal class Tokenizer
    {
        internal readonly TextReader Reader;
        private string _currentLine;
        private int _ich;

        internal Tokenizer(TextReader reader)
        {
            Reader = reader;
            // ReSharper disable once PossibleNullReferenceException
            _currentLine = reader.ReadLine().Trim();
        }

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
	}
}
