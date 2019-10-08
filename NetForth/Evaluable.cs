namespace NetForth
{
    internal abstract class Evaluable
    {
        internal enum ExitType
        {
            Leave,
            Exit
        }

        internal bool IsImmediate { get; set; }
        protected Evaluable _parent;

        internal virtual void Eval(Evaluable parent = null, WordListBuilder wlb = null)
        {
            _parent = parent;
            InnerEval(wlb);
        }

        internal abstract void InnerEval(WordListBuilder wlb);

        internal virtual void  Leave(ExitType exitType)
        {
            _parent?.Leave(exitType);
        }
    }
}
