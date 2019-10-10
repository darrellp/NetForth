namespace NetForth
{
    internal class IntPrim : Evaluable
    {
        private readonly int _value;

        internal IntPrim(int value)
        {
            _value = value;
        }

        protected override void InnerEval(WordListBuilder wlb)
        {
            DataStack.Stack.Push(_value);
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
