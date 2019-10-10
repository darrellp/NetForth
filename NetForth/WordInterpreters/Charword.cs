namespace NetForth.WordInterpreters
{
    internal class CharWord : WordInterpreter
    {
        private readonly WordListBuilder _wlbParent;

        internal CharWord(WordListBuilder wlb)
        {
            _wlbParent = wlb;
        }

        internal override void InterpretWord(string word)
        {
            _wlbParent.Add(new IntPrim((int)word[0]));
            Interpreter.InterpreterStack.Pop();
        }
	}
}
