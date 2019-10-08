namespace NetForth
{
    internal class IntPrim : Evaluable
    {
        private readonly int _value;

        internal IntPrim(int value)
        {
            _value = value;
        }

        internal override void InnerEval(WordListBuilder wlb = null)
        {
            DataStack.Stack.Push(_value);
        }
    }
}
