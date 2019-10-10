namespace NetForth.WordInterpreters
{
    internal class CCharWord : WordInterpreter
    {
        private readonly WordListBuilder _wlbParent;

        internal CCharWord(WordListBuilder wlb)
        {
            _wlbParent = wlb;
        }

        internal override void InterpretWord(string word)
        {
            _wlbParent.Add(new IntPrim((int)word[0]));
            Interpreter.InterpreterStack.Pop();
        }
	}

    internal class CharWord : WordInterpreter
    {
        internal override void InterpretWord(string word)
        {
            DataStack.Stack.Push((int)word[0]);
            Interpreter.InterpreterStack.Pop();
        }
    }
}
