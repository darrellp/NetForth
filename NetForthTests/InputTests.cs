using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetForth;
using FluentAssertions;
using static NetForth.Session;

namespace NetForthTests
{
    [TestClass]
    public class  InputTests
    {
        private Session session;

        [TestInitialize]
        public void TestInitialize()
        {
            session = new Session();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            session.Dispose();
        }

        [TestMethod]
        public void KeyTests()
        {
            TestScript("key", (int) 'a');
            TestScript("key", (int)'b');
        }

        private void TestScript(string script, int expected)
        {
            var intrp = new Interpreter();
            try
            {
                intrp.Interpret(script);
                Stack.Should().HaveCount(1);
                Stack[0].Should().Be(expected);
            }
            finally
            {
                Stack.Clear();
            }
        }

	}
}