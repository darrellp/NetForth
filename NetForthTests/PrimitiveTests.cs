using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetForth;
using FluentAssertions;
using static NetForth.Session;

namespace NetForthTests
{
	[TestClass]
	public class PrimitiveTests
	{
		[TestMethod]
		public void TestPrimitiveCreation()
        {
            var val = 0;
            var test = new Primitive(() => val = 5, "");
            test.Eval();
            val.Should().Be(5, "because the primitive should have set it");
        }

        [TestMethod]
        public void TestIntPrim()
        {
            Stack.Clear();
            var intPrim = new IntPrim(13);
            intPrim.Eval();
            var val = Stack.Pop();
            val.Should().Be(13);
            Stack.Should().BeEmpty();
        }
	}
}
