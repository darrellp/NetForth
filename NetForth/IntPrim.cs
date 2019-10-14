namespace NetForth
{
    internal class IntPrim : Evaluable
    {
        private readonly string _name;

        internal int Value { get; }

        internal IntPrim(int value, string name = null)
        {
            Value = value;
            _name = name;
        }

        internal override ExitType Eval(Tokenizer tokenizer)
        {
			DataStack.Stack.Push(Value);
            return ExitType.Okay;
        }

		public override string ToString()
        {
            return _name != null ? $"{_name}({Value})" : Value.ToString();
        }
    }
}
