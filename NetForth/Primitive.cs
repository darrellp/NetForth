using System;

namespace NetForth
{
	internal class Primitive : Evaluable
    {
        private readonly Action _action;
        private readonly Action<WordListBuilder> _wordListAction;

        internal Primitive(Action action, bool isImmediate = false)
        {
            _action = action;
            IsImmediate = isImmediate;
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
    }
}
