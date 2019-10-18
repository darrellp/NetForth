using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NetForth
{
	internal class Session : IDisposable
    {
		#region Statics
		internal static readonly ForthStack<int> Stack = new ForthStack<int>();
        internal static IntPtr ForthMemory;
        internal static int CbMemory;
        internal static int FreeOffset;
        internal static string LastDefinedWord;
        internal static readonly ForthStack<int> ReturnStack = new ForthStack<int>();
        internal static bool IsTesting;
        internal static readonly List<Evaluable> EvaluableVals = new List<Evaluable>();
        internal static readonly Dictionary<string, int> MapForthWordToIndex = new Dictionary<string, int>();
		internal static readonly Dictionary<string, Delegate> MapNameToDNetFn = new Dictionary<string, Delegate>();
        internal static readonly Dictionary<string, int> MapNameToObjectIndex = new Dictionary<string, int>();
        internal static readonly List<object> DotNetObjects = new List<object>();
#if SMALLSTRINGS
        public static readonly int StringLengthSize = 1;
#else
        public static readonly int StringLengthSize = sizeof(int);
#endif
		#endregion

		#region Constructor
		public Session(int cbMemory = 5000)
        {
            CbMemory = cbMemory;
            ForthMemory = Marshal.AllocHGlobal(CbMemory);
            FreeOffset = 0;
            Vocabulary.Init();
            ReturnStack.Clear();
            EvaluableVals.Clear();
            MapForthWordToIndex.Clear();
            EvaluableVals.Clear();
            MapNameToDNetFn.Clear();
            MapNameToObjectIndex.Clear();
            DotNetObjects.Clear();

            StackTrace stackTrace = new StackTrace();           // get call stack
            StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

            // write call stack method names
            foreach (StackFrame stackFrame in stackFrames)
            {
                var name = stackFrame.GetMethod().Module.Name;

				IsTesting = name.Contains("NetForthTests");
                if (IsTesting)
                {
                    return;
                }
            }
        }
		#endregion

		#region .NET interaction
        public static void AddDotNetFn(string name, Delegate myDelegate)
        {
            MapNameToDNetFn[name] = myDelegate;
        }

        public static int SaveManagedObject(Object obj)
        {
            var iRet = DotNetObjects.Count;
            DotNetObjects.Add(obj);
            return iRet;
        }

        public static void Invoke(string name)
        {
            if (!MapNameToDNetFn.ContainsKey(name))
            {
                throw new NfException($"Trying to invoke non-existent function {name}");
            }

            var del = MapNameToDNetFn[name];
            var methodInfo = del.Method;

			var tRet = methodInfo.ReturnType;
            var tParms = methodInfo.GetParameters().Select(pi => pi.ParameterType).ToArray();

            var passedParms = new object[tParms.Length];
            for (var i = tParms.Length - 1; i >= 0; i--)
            {
                if (tParms[i] == typeof(int))
                {
                    passedParms[i] = Stack.Pop();
                    continue;
                }

                var index = Stack.Pop();
                passedParms[i] = DotNetObjects[index];
            }
            
            object result = del.DynamicInvoke(passedParms);
            if (tRet == typeof(void))
            {
                return;
            }
            if (tRet == typeof(int))
            {
                Stack.Push((int)result);
                return;
            }
            Stack.Push(SaveManagedObject(result));
        }
		#endregion

		#region IDispose
		private void ReleaseUnmanagedResources()
        {
			Marshal.FreeHGlobal(ForthMemory);
            ForthMemory = (IntPtr) 0;
		}

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Session()
        {
            ReleaseUnmanagedResources();
        }
		#endregion
	}
}
