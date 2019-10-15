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
		//internal virtual ExitType Eval()
  //      {
  //          return Eval(null);
  //      }

        internal virtual ExitType Eval(Tokenizer tokenizer = null)
        {
            return Eval(tokenizer, null);
        }

        internal virtual ExitType Eval(Tokenizer tokenizer, WordListBuilder wlb)
        {
            return ExitType.Okay;
        }
        #endregion
    }
}
