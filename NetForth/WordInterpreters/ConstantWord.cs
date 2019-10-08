namespace NetForth.WordInterpreters
{
    internal class ConstantWord : WordInterpreter
	{
        internal override void InterpretWord(string word)
        {
            Vocabulary.CurrentVocabulary.AddDefinition(word, new IntPrim(DataStack.Stack.Pop()));
            Interpreter.InterpreterStack.Pop();
        }
	}
}
