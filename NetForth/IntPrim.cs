namespace NetForth
{
    internal class IntPrim : Evaluable
    {
        private readonly int _value;
        private readonly string _name;

        internal IntPrim(int value, string name = null)
        {
            _value = value;
            _name = name;
        }

        protected override void InnerEval(WordListBuilder wlb)
        {
            DataStack.Stack.Push(_value);
        }

        public override string ToString()
        {
            return _name != null ? $"{_name}({_value})" : _value.ToString();
        }
    }
}
