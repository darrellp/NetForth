namespace NetForth.WordInterpreters
{
    internal class CreateWord : WordInterpreter
    {
        internal override void InterpretWord(string word)
        {
            var lcWord = word.ToLower();
            Session.LastDefinedWord = lcWord;
			Vocabulary.CurrentVocabulary.AddDefinition(lcWord, new IntPrim(Memory.Here(), lcWord));
            Interpreter.InterpreterStack.Pop();
        }
    }
}