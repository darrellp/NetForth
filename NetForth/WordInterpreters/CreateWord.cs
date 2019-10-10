namespace NetForth.WordInterpreters
{
    internal class CreateWord : WordInterpreter
    {
        internal override void InterpretWord(string word)
        {
            Vocabulary.CurrentVocabulary.AddDefinition(word, new IntPrim(Memory.Here(), word));
            Interpreter.InterpreterStack.Pop();
        }
    }
}