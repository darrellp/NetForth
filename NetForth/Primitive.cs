using System;

namespace NetForth
{
    // Primitives that must compile code and place a resulting primitive in their parent's word list
    // (i.e., but, do and most flow of control words)
    internal class Compilable : Evaluable
    {
        private readonly Action<Tokenizer, WordListBuilder> _compileAction;

        internal override ExitType Eval(Tokenizer tokenizer = null, WordListBuilder wlb = null)
        {
            _compileAction(tokenizer, wlb);
            return ExitType.Okay;
        }

        internal Compilable(Action<Tokenizer, WordListBuilder> action)
        {
            _compileAction = action;
            IsImmediate = true;
        }
    }

    // Primitives that must scan ahead at upcoming words in the stream but do not
    // compile anything into the parent's evolving word list (i.e., Constant, Create, etc.)
	internal class LookAhead : Evaluable
    {
        private readonly Action<Tokenizer> _action;
        internal string Name { get; }

        internal LookAhead(Action<Tokenizer> action, string name, bool isImmediate = false)
        {
            _action = action;
            IsImmediate = isImmediate;
            Name = name;
        }

        internal override ExitType Eval(Tokenizer tokenizer = null, WordListBuilder _ = null)
        {
            _action(tokenizer);
            return ExitType.Okay;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    internal class ThrowPrimitive : Evaluable
    {
        private readonly Func<ExitType> _action;
        private string Name { get; }

        internal ThrowPrimitive(Func<ExitType> action, string name, bool isImmediate = false)
        {
            _action = action;
            IsImmediate = isImmediate;
            Name = name;
        }

        internal override ExitType Eval(Tokenizer _ = null, WordListBuilder wlb = null)
        {
            return _action();
        }

        public override string ToString()
        {
            return Name;
        }
    }

    internal class NewPrimitive : Evaluable
    {
        private readonly Func<ExitType> _action;
        internal string Name { get; }

		internal NewPrimitive(Action action, string name, bool isImmediate = false)
		{
			_action = () =>
			{
				action();
				return ExitType.Okay;
			};
			Name = name;
			IsImmediate = isImmediate;
		}

        internal override ExitType Eval(Tokenizer tokenizer = null, WordListBuilder wlb = null)
        {
            _action();
            return ExitType.Okay;
        }

		//internal override ExitType Eval()
  //      {
  //          return _action();
  //      }

        public override string ToString()
        {
            return Name;
        }
    }
}
