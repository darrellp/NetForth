using System;
using System.Collections.Generic;
using NetForth.WordInterpreters;
using static NetForth.DataStack;
using static NetForth.Evaluable;

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
                {"dup", new NewPrimitive(dup, "dup")},
                {"+", new NewPrimitive(plus, "+")},
                {"-", new NewPrimitive(minus, "-")},
				{"*", new NewPrimitive(times, "*")},
				{"/", new NewPrimitive(divide, "/")},
                {".", new NewPrimitive(dot, ".")},
                {".s", new NewPrimitive(dotS, ".s")},
                {"?dup", new NewPrimitive(qDup, "?dup")},
                {"drop", new NewPrimitive(drop, "drop")},
                {"swap", new NewPrimitive(swap, "swap")},
                {"over", new NewPrimitive(over, "over")},
				{"nip", new NewPrimitive(nip, "nip")},
                {"tuck", new NewPrimitive(tuck, "tuck")},
                {"rot", new NewPrimitive(rot, "rot")},
				{"-rot", new NewPrimitive(minusRot, "-rot")},
                {"pick", new NewPrimitive(pick, "pick")},
                {"2dup", new NewPrimitive(dup2, "2dup")},
                {"2drop", new NewPrimitive(drop2, "2drop")},
                {"2swap", new NewPrimitive(swap2, "2swap")},
                {"2over", new NewPrimitive(over2, "2over")},
                {"um*", new NewPrimitive(umStar, "um*")},
                {"*/", new NewPrimitive(timesDiv, "*/")},
                {"mod", new NewPrimitive(mod, "mod")},
                {"um/mod", new NewPrimitive(umMod, "um/mod")},
                {"negate", new NewPrimitive(negate, "negate")},
                {"abs", new NewPrimitive(abs, "abs")},
                {"min", new NewPrimitive(min, "min")},
                {"max", new NewPrimitive(max, "max")},
                {"\\", new LookAhead(lineComment, "\\")},
                {"(", new LookAhead(mlComment, "(", true)},
                {":", new LookAhead(define, ":") },
                {"<", new NewPrimitive(lt, "<") },
                {">", new NewPrimitive(gt, ">") },
                {"<=", new NewPrimitive(leq, "<=") },
                {">=", new NewPrimitive(geq, ">=") },
                {"u<", new NewPrimitive(ult, "u<") },
                {"u>", new NewPrimitive(ugt, "u>") },
                {"=", new NewPrimitive(eq, "=") },
                {"<>", new NewPrimitive(neq, "<>") },
                {"0=", new NewPrimitive(zeq, "0=") },
                {"0<>", new NewPrimitive(zneq, "0<>") },
                {"and", new NewPrimitive(and, "and") },
                {"or", new NewPrimitive(or, "or") },
                {"xor", new NewPrimitive(xor, "xor") },
                {"invert", new NewPrimitive(invert, "invert") },
                {"lshift", new NewPrimitive(lshift, "lshift") },
                {"rshift", new NewPrimitive(rshift, "rshift") },
                {"constant", new LookAhead(constant, "constant") },
                {"variable", new LookAhead(variable, "variable") },
                {"value", new LookAhead(value, "value") },
                {"@", new NewPrimitive(fetch, "@") },
                {"!", new NewPrimitive(store, "!") },
                {"c@", new NewPrimitive(cFetch, "c@") },
                {"c!", new NewPrimitive(cStore, "c!") },
                {"w@", new NewPrimitive(wFetch, "w@") },
                {"w!", new NewPrimitive(wStore, "w!") },
                {"+!", new NewPrimitive(memAdd, "+!") },
                {"cells", new NewPrimitive(cells, "cells") },
                {"chars", new NewPrimitive(chars, "chars") },
                {"if", new Compilable(iffn) },
                {"do", new Compilable(dofn) },
				{"?do", new Compilable(questDofn) },
                {"begin", new Compilable(begin) },
				{"i", new NewPrimitive(i, "i") },
				{"j", new NewPrimitive(j, "j") },
				{"leave", new ThrowPrimitive(leave, "leave") },
				{"?leave", new ThrowPrimitive(condLeave, "?leave") },
                {"exit", new ThrowPrimitive(exit, "exit") },
				{"c\"", new Compilable(countedString) },
                {"[char]", new LookAhead(fromChar, "[char]", true) },
                {"char", new LookAhead(fromChar, "char") },
                {"create", new LookAhead(create, "create") },
                {",", new NewPrimitive(comma, ",") },
                {"c,", new NewPrimitive(charComma, "c,") },
                {"here", new NewPrimitive(here, "here") },
                {"allot", new NewPrimitive(allot, "allot") },
			};

            Vocabulary.AddVocabulary(new Vocabulary(rootPrimitives, "Root"));
        }
		#endregion

		#region Strings
		private static void countedString(Tokenizer tokenizer, WordListBuilder wlb)
		{
			FStringAction.FString(tokenizer, wlb);
		}

		private static void fromChar(Tokenizer tokenizer)
        {
            var word = tokenizer.NextToken();
            Stack.Push((int)word[0]);
        }
		#endregion

		#region Flow of Control
		private static void iffn(Tokenizer tokenizer, WordListBuilder wlb)
        {
            IfAction.If(tokenizer, wlb);
        }

        private static void dofn(Tokenizer tokenizer, WordListBuilder wlb)
        {
            DoAction.Do(tokenizer, wlb, false);
        }

		private static void questDofn(Tokenizer tokenizer, WordListBuilder wlb)
		{
            DoAction.Do(tokenizer, wlb, true);
		}

		private static void begin(Tokenizer tokenizer, WordListBuilder wlb)
        {
            BeginAction.Begin(tokenizer, wlb);
        }

		private static void i()
		{
			try
			{
				Stack.Push(DoAction.i());
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
				Stack.Push(DoAction.j());
			}
			catch (NfException)
			{
				throw new NfException("Trying to retrieve J outside of nested loop");
			}
		}

		private static ExitType leave()
        {
            return ExitType.Leave;
        }

		private static ExitType condLeave()
		{
			if (Stack.Pop() != 0)
			{
				return leave();
			}

            return ExitType.Okay;
        }

        private static ExitType exit()
        {
            return ExitType.Exit;
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

        private static void comma()
        {
            Memory.StoreInt(Memory.Allocate(), Stack.Pop());
        }

        private static void charComma()
        {
            Memory.StoreChar(Memory.Allocate(sizeof(char)), (char)Stack.Pop());
        }

        private static void here()
        {
            Stack.Push(Memory.Here());
        }

        private static void allot()
        {
            Memory.Allocate(Stack.Pop());
        }

        private static void constant(Tokenizer tokenizer)
        {
            var word = tokenizer.NextToken().ToLower();
            Vocabulary.CurrentVocabulary.AddDefinition(word, new IntPrim(DataStack.Stack.Pop(), word));
        }

		private static void variable(Tokenizer tokenizer)
        {
            var word = tokenizer.NextToken().ToLower();
            var address = Memory.Allocate();
            Memory.StoreInt(address, 0);
            Vocabulary.CurrentVocabulary.AddDefinition(word, new IntPrim(address, word));
        }

		private static void value(Tokenizer tokenizer)
        {
            var word = tokenizer.NextToken().ToLower();
            var address = Memory.Allocate();
            Memory.StoreInt(address, Stack.Pop());
            Vocabulary.CurrentVocabulary.AddDefinition(word, new IntPrim(address, word));
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

        private static void create(Tokenizer tokenizer)
        {
            var lcWord = tokenizer.NextToken().ToLower();
            Session.LastDefinedWord = lcWord;
            Vocabulary.CurrentVocabulary.AddDefinition(lcWord, new IntPrim(Memory.Here(), lcWord));
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
		private static void lineComment(Tokenizer tokenizer)
        {
            while (tokenizer.NextToken(true, true) != "\n") ;
        }

        private static void mlComment(Tokenizer tokenizer)
        {
            while (tokenizer.NextToken() != ")") ;
        }
		#endregion

		#region Compiling
        private static void define(Tokenizer tokenizer)
        {
            Definition.ParseDefinition(tokenizer);
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
