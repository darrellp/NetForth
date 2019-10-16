using static NetForth.Session;

namespace NetForth
{
    internal class IntPrim : Evaluable
    {
		#region Private variables
		private readonly string _name;
		#endregion

		#region Public properties
		internal int Value { get; }
		#endregion

		#region Constructor
		internal IntPrim(int value, string name = null)
        {
            Value = value;
            _name = name;
        }
		#endregion

		#region Evaluation
		internal override ExitType Eval(Tokenizer tokenizer = null, WordListBuilder wlb = null)
        {
			Stack.Push(Value);
            return ExitType.Okay;
        }
		#endregion

		#region overrides
		public override string ToString()
        {
            return _name != null ? $"{_name}({Value})" : Value.ToString();
        }
		#endregion
	}
}
