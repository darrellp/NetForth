using System;
using System.Collections.Generic;
using NetForth.WordInterpreters;
using static NetForth.DataStack;

// ReSharper disable InconsistentNaming

namespace NetForth
{
#pragma warning disable IDE1006 // Naming Styles
    internal static class RootPrims
    {
		#region Setting up root vocabulary
		internal static void AddRoot()
        {
            var rootPrimitives = new Dictionary<string, Evaluable>()
            {
                {"dup", new Primitive(dup)},
                {"+", new Primitive(plus)},
                {"-", new Primitive(minus)},
				{"*", new Primitive(times)},
				{"/", new Primitive(divide)},
                {".", new Primitive(dot)},
                {".s", new Primitive(dotS)},
                {"?dup", new Primitive(qDup)},
                {"drop", new Primitive(drop)},
                {"swap", new Primitive(swap)},
                {"over", new Primitive(over)},
                {"nip", new Primitive(nip)},
                {"tuck", new Primitive(tuck)},
                {"rot", new Primitive(rot)},
				{"-rot", new Primitive(minusRot)},
                {"pick", new Primitive(pick)},
                {"2dup", new Primitive(dup2)},
                {"2drop", new Primitive(drop2)},
                {"2swap", new Primitive(swap2)},
                {"2over", new Primitive(over2)},
                {"um*", new Primitive(umStar)},
                {"*/", new Primitive(timesDiv)},
                {"mod", new Primitive(mod)},
                {"um/mod", new Primitive(umMod)},
                {"negate", new Primitive(negate)},
                {"abs", new Primitive(abs)},
                {"min", new Primitive(min)},
                {"max", new Primitive(max)},
                {"\\", new Primitive(lineComment)},
                {"(", new Primitive(mlComment, true)},
                {":", new Primitive(define) },
                {"<", new Primitive(lt) },
                {">", new Primitive(gt) },
                {"<=", new Primitive(leq) },
                {">=", new Primitive(geq) },
                {"u<", new Primitive(ult) },
                {"u>", new Primitive(ugt) },
                {"=", new Primitive(eq) },
                {"<>", new Primitive(neq) },
                {"0=", new Primitive(zeq) },
                {"0<>", new Primitive(zneq) },
                {"and", new Primitive(and) },
                {"or", new Primitive(or) },
                {"xor", new Primitive(xor) },
                {"invert", new Primitive(invert) },
                {"lshift", new Primitive(lshift) },
                {"rshift", new Primitive(rshift) },
                {"constant", new Primitive(constant) },
                {"variable", new Primitive(variable) },
                {"value", new Primitive(value) },
                {"@", new Primitive(fetch) },
                {"!", new Primitive(store) },
                {"c@", new Primitive(cFetch) },
                {"c!", new Primitive(cStore) },
                {"w@", new Primitive(wFetch) },
                {"w!", new Primitive(wStore) },
                {"+!", new Primitive(memAdd) },
                {"cells", new Primitive(cells) },
                {"chars", new Primitive(chars) },
                {"if", new Primitive(iffn, true) },
                {"do", new Primitive(dofn, true) },
				{"?do", new Primitive(questDofn, true) },
                {"begin", new Primitive(begin, true) },
				{"i", new Primitive(i) },
				{"j", new Primitive(j) },
				{"leave", new Primitive(leave) },
				{"?leave", new Primitive(condLeave) },
                {"exit", new Primitive(exit) },
				{"c\"", new Primitive(countedString, true) },
			};

            Vocabulary.AddVocabulary(new Vocabulary(rootPrimitives));
        }
		#endregion

		#region Strings
		private static void countedString(WordListBuilder wlb)
		{
			Interpreter.InterpreterStack.Push(new FString(wlb));
		}
		#endregion

		#region Flow of Control
		private static void iffn(WordListBuilder wlb)
        {
            Interpreter.InterpreterStack.Push(new IfWord(wlb));
        }

        private static void dofn(WordListBuilder wlb)
        {
            Interpreter.InterpreterStack.Push(new DoWord(wlb));
        }

		private static void questDofn(WordListBuilder wlb)
		{
			Interpreter.InterpreterStack.Push(new DoWord(wlb, true));
		}

        private static void begin(WordListBuilder wlb)
        {
            Interpreter.InterpreterStack.Push(new BeginWord(wlb));
        }

		private static void i()
		{
			try
			{
				Stack.Push(DoWord.i());
			}
			catch (NfException)
			{
				throw new NfException("Trying to retrieve I outside of loop");
			}
		}

		private static void j()
		{
			try
			{
				Stack.Push(DoWord.j());
			}
			catch (NfException)
			{
				throw new NfException("Trying to retrieve J outside of nested loop");
			}
		}

		private static void leave()
        {
            Session.RunningPrimitive.Leave(Evaluable.ExitType.Leave);
        }

		private static void condLeave()
		{
			if (Stack.Pop() != 0)
			{
				leave();
			}
		}

        private static void exit()
        {
            Session.RunningPrimitive.Leave(Evaluable.ExitType.Exit);
        }
		#endregion

		#region Memory
		private static void cells()
        {
            Stack[-1] *= 4;
        }

        private static void chars()
        {
            Stack[-1] *= 2;
        }

        private static void constant()
        {
            Interpreter.InterpreterStack.Push(new ConstantWord());
        }

        private static void variable()
        {
            Interpreter.InterpreterStack.Push(new VariableWord());
        }

        private static void value()
        {
            Interpreter.InterpreterStack.Push(new ValueWord());
        }

        private static void fetch()
        {
            Stack[-1] = Memory.FetchInt(Stack[-1]);
        }

        private static void store()
        {
            Memory.StoreInt(Stack.Pop(), Stack.Pop());
        }

        private static void cFetch()
        {
            Stack[-1] = Memory.FetchUShort(Stack[-1]);
        }

        private static void cStore()
        {
            Memory.StoreUShort(Stack.Pop(), (ushort)Stack.Pop());
        }

        private static void wFetch()
        {
            Stack[-1] = Memory.FetchShort(Stack[-1]);
        }

        private static void wStore()
        {
            Memory.StoreShort(Stack.Pop(), (short)Stack.Pop());
        }

		private static void memAdd()
        {
            var address = Stack.Pop();
            var delta = Stack.Pop();
            Memory.StoreInt(address, Memory.FetchInt(address) + delta);
        }
		#endregion

		#region Logic operations
		private static void and()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] & n2;
        }

        private static void or()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] | n2;
        }

        private static void xor()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] ^ n2;
        }

        private static void invert()
		{
            Stack[-1] = ~Stack[-1];
        }

        private static void lshift()
        {
            var u = Stack.Pop();
            Stack[-1] = Stack[-1] << u;
        }

        private static void rshift()
        {
            var u = Stack.Pop();
            Stack[-1] = Stack[-1] >> u;
        }
		#endregion

		#region Comparisons
		private static void eq()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] == n2 ? -1 : 0;
        }

        private static void neq()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] != n2 ? -1 : 0;
        }

        private static void leq()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] <= n2 ? -1 : 0;
        }

		private static void geq()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] >= n2 ? -1 : 0;
        }

        private static void zeq()
        {
            Stack[-1] = Stack[-1] == 0 ? -1 : 0;
        }

		private static void zneq()
        {
            Stack[-1] = Stack[-1] != 0 ? -1 : 0;
        }

        private static void lt()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] < n2 ? -1 : 0;
        }

        private static void gt()
        {
            var n2 = Stack.Pop();
            Stack[-1] = Stack[-1] > n2 ? -1 : 0;
        }

        private static void ult()
        {
            var n2 = (uint)Stack.Pop();
            Stack[-1] = (uint)Stack[-1] < n2 ? -1 : 0;
        }

        private static void ugt()
        {
            var n2 = (uint)Stack.Pop();
            Stack[-1] = (uint)Stack[-1] > n2 ? -1 : 0;
        }
		#endregion

		#region Comments
		private static void lineComment()
        {
            Interpreter.InterpreterStack.Push(new CommentWord(true));
        }

        private static void mlComment()
        {
            Interpreter.InterpreterStack.Push(new CommentWord());
        }
		#endregion

		#region Compiling
        private static void define()
        {
            Interpreter.InterpreterStack.Push(new DefinitionWord());
        }
		#endregion

		#region Stack manipulation
		private static void over2()
        {
            Stack.Push(Stack[-4]);
            Stack.Push(Stack[-4]);
        }

		private static void swap2()
        {
            (Stack[-4], Stack[-3], Stack[-2], Stack[-1]) =
                (Stack[-2], Stack[-1], Stack[-4], Stack[-3]);
        }

        private static void drop2()
        {
            Stack.Pop();
            Stack.Pop();
        }

        private static void dup2()
        {
            Stack.Data.Add(Stack[-2]);
            Stack.Data.Add(Stack[-2]);
        }

		private static void pick()
        {
            var index = -(Stack.Pop() + 1);
            Stack.Push(Stack[index]);
        }

        private static void rot()
        {
            (Stack[-3], Stack[-2], Stack[-1]) = (Stack[-2], Stack[-1], Stack[-3]);
        }

        private static void minusRot()
        {
            (Stack[-3], Stack[-2], Stack[-1]) = (Stack[-1], Stack[-3], Stack[-2]);
        }

		private static void tuck()
        {
            swap();
            over();
        }
        
        private static void nip()
        {
            Stack.Data.RemoveAt(Stack.Data.Count - 2);
        }

		private static void over()
        {
            Stack.Data.Add(Stack[-2]);
        }

		private static void swap()
        {
            (Stack[-1], Stack[-2]) = (Stack[-2], Stack[-1]);
        }

        private static void drop()
        {
            Stack.Pop();
        }

        private static void dup()
        {
            Stack.Data.Add(Stack[-1]);
        }

		private static void qDup()
		{
            if (Stack.Peek() != 0)
            {
                dup();
            }
        }
		#endregion

		#region Arithmetic
        private static void min()
        {
            var v = Stack.Pop();
            Stack[-1] = Math.Min(v, Stack[-1]);
        }

        private static void max()
        {
            var v = Stack.Pop();
            Stack[-1] = Math.Max(v, Stack[-1]);
        }
        
        private static void abs()
        {
            Stack[-1] = Math.Abs(Stack[-1]);
        }

		private static void plus()
        {
            var a1 = Stack.Pop();
            Stack[-1] += a1;
        }

        private static void minus()
        {
            var a1 = Stack.Pop();
            Stack[-1] -= a1;
        }

        private static void times()
        {
            var a1 = Stack.Pop();
            Stack[-1] *= a1;
        }

        private static void divide()
        {
            var a1 = Stack.Pop();
            Stack[-1] /= a1;
        }

        private static void mod()
        {
            var a1 = Stack.Pop();
            Stack[-1] = Stack[-1] % a1;
        }

        private static void umStar()
        {
            var v = (ulong) Stack[-1] * (ulong) Stack[-2];
            Stack[-2] = (int)v;
            Stack[-1] = (int)(v >> 16);
        }

        private static void timesDiv()
        {
            var v = Stack.Pop() * (long)Stack.Pop();
            Stack[-1] = (int)(v / Stack[-1]);
        }

        private static void umMod()
        {
            var denominator = Stack.Pop();
            var numerator = Stack.Pop() + ((long) Stack.Pop()) << 16;
            Stack.Push((int)(numerator % denominator));
            Stack.Push((int)(numerator / denominator));
        }

        private static void negate()
        {
            Stack[-1] = -Stack[-1];
        }
		#endregion

		#region I/O
		private static void dot()
        {
            Console.Write($" {Stack.Pop()}");
        }

        private static void dotS()
        {
            Console.Write($"<{Stack.Count}>");
            foreach (var val in Stack)
            {
                Console.Write($" {val}");
            }
        }
		#endregion
	}
}
