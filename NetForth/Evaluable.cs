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
		internal virtual ExitType NewEval()
        {
            return NewEval(null);
        }

        internal virtual ExitType NewEval(Tokenizer tokenizer)
        {
            return NewEval(tokenizer, null);
        }

        internal virtual ExitType NewEval(Tokenizer tokenizer, WordListBuilder wlb)
        {
            return ExitType.Okay;
        }
        #endregion
    }
}
