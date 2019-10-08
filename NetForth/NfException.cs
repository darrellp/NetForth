using System;

namespace NetForth
{
	class NfException : Exception
	{
		public NfException(string str) : base(str) { }
	}
}
