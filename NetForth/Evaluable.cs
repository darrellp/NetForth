namespace NetForth
{
    internal abstract class Evaluable
    {
		#region Enums
		internal enum ExitType
        {
            Okay,
            Leave,
            Exit,
        }
		#endregion

		#region Public properties
		internal bool IsImmediate { get; set; }
		#endregion

		#region Evaluation
        // TODO: Consider making tokenizer and WordListBuilder static and taking them out as parameters in Eval for performance
        internal virtual ExitType Eval(Tokenizer tokenizer = null, WordListBuilder wlb = null)
        {
            return ExitType.Okay;
        }
        #endregion
    }
}
