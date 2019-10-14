using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetForth;
using FluentAssertions;
using static NetForth.DataStack;

namespace NetForthTests
{
	[TestClass]
	public class MemoryTests
    {
        [TestMethod]
        public void TestVariable()
        {
            using (var nf = new Session())
            {
                var intrp = new Interpreter("variable doggy 20 doggy ! doggy @");
                intrp.Interpret();
                Stack.Should().HaveCount(1);
                Stack[0].Should().Be(20);
                Stack.Clear();
            }
        }

        [TestMethod]
        public void TestMemAdd()
        {
            using (var nf = new Session())
            {
                var intrp = new Interpreter("variable doggy 20 doggy ! 10 doggy +! doggy @");
                intrp.Interpret();
                Stack.Should().HaveCount(1);
                Stack[0].Should().Be(30);
                Stack.Clear();
            }
        }
	}
}
