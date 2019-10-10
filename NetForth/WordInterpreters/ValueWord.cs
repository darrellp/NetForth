namespace NetForth.WordInterpreters
{
	class ValueWord : WordInterpreter
	{
		internal override void InterpretWord(string word)
        {
            var address = Memory.Allocate();
            Memory.StoreInt(address, DataStack.Stack.Pop());
            Vocabulary.CurrentVocabulary.AddDefinition(word, new IntPrim(address, word));
            Interpreter.InterpreterStack.Pop();
        }
	}
}
