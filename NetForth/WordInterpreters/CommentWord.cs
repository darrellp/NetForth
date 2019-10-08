namespace NetForth.WordInterpreters
{
	class CommentWord : WordInterpreter
    {
        private readonly bool _isLineComment;

        internal CommentWord(bool isLineComment = false)
        {
            _isLineComment = isLineComment;
        }

        internal override void InterpretWord(string word)
        {
            if (_isLineComment ? word == "\n" : word == ")")
            {
                Interpreter.InterpreterStack.Pop();
            }
        }
    }
}
