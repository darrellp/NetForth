using System;

namespace NetForth
{
	internal class Primitive : Evaluable
    {
        private readonly Action _action;
        private readonly Action<WordListBuilder> _wordListAction;
        internal string Name { get; }

        internal Primitive(Action action, string name, bool isImmediate = false)
        {
            _action = action;
            IsImmediate = isImmediate;
            Name = name;
        }

        internal Primitive(Action<WordListBuilder> action, bool isImmediate = false)
        {
            _wordListAction = action;
            IsImmediate = isImmediate;
        }

        protected override void InnerEval(WordListBuilder wlb)
        {
            if (_action != null)
            {
                Session.RunningPrimitive = this;
                _action();
                Session.RunningPrimitive = null;
            }
			else
			{
                 _wordListAction(wlb);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
