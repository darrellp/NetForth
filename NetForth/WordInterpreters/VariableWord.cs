namespace NetForth.WordInterpreters
{
	class VariableWord : WordInterpreter
	{
		internal override void InterpretWord(string word)
        {
            var address = Memory.Allocate();
            Memory.StoreInt(address, 0);
            Vocabulary.CurrentVocabulary.AddDefinition(word, new IntPrim(address));
            Interpreter.InterpreterStack.Pop();
        }
	}
}
