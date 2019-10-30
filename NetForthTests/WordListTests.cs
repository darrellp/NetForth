using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NetForth;
using NetForth.Primitives;

namespace NetForthTests
{
	[TestClass]
	public class WordListTests
	{
		[TestMethod]
		public void TestWordListCreation()
		{
            var val1 = 0;
            var val2 = 0;
            var test1 = new Primitive(() => val1 = 5, "");
            var test2 = new Primitive(() => val2 = 10, "");
            var wordList = new WordList(new List<Evaluable>() { test1, test2});
            wordList.Eval();
            val1.Should().Be(5);
            val2.Should().Be(10);
        }

        [TestMethod]
        public void TestDefinition()
        {
            var intPrim10 = new IntPrim(10);
            var intPrim20 = new IntPrim(20);
            var plus = Vocabulary.Lookup("+");
            var dup = Vocabulary.Lookup("Dup");
            plus.Should().NotBeNull();
            dup.Should().NotBeNull();
            // Should be 20 + 2 * 10 = 40.
            var def = new WordList("", intPrim20, intPrim10, dup, plus, plus);
            def.Eval();
            var val = Session.Stack.Pop();
            Session.Stack.Should().BeEmpty();
            val.Should().Be(40, "because 20 + 2 * 10 = 40");
        }
	}
}
