using System;

namespace NetForth.Primitives
{
    internal class DotNetPrimitive : Evaluable
    {
        protected Type TRet;
        protected Type[] TParms;

        protected DotNetPrimitive(Type tRet, Type[] tParms)
        {
            TRet = tRet;
            TParms = tParms;
        }

        protected DotNetPrimitive() { }

        protected object[] GetPassedParms()
        {
            var passedParms = new object[TParms.Length];
            for (var i = TParms.Length - 1; i >= 0; i--)
            {
                var val = Session.Stack.Pop();
                passedParms[i] = TParms[i] == typeof(int) ? val : Session.DotNetObjects[val];
            }

            return passedParms;
        }
    }
}