using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetForth;
using FluentAssertions;

namespace NetForthTests
{
	[TestClass]
	public class OutputTests
    {
        private StringWriter _swrit;
        private Session session;

        [TestInitialize]
        public void TestInit()
        {
            _swrit = new StringWriter();
            Console.SetOut(_swrit);
            session = new Session();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            var standardOutput = new StreamWriter(Console.OpenStandardOutput()) {AutoFlush = true};
            Console.SetOut(standardOutput);
            session.Dispose();
        }

		[TestMethod]
		public void TestOutput()
		{
            Console.Write("Darrell Plank");
            _swrit.ToString().Should().Be("Darrell Plank");
        }

        [TestMethod]
        public void TestDot()
        {
            Interpret("10 .");
            _swrit.ToString().Should().Be("10");
        }

        [TestMethod]
        public void TestDotS()
        {
            Interpret(".s");
            _swrit.ToString().Should().Be("<0>");
            Reset();
            Interpret("10 5 .s");
            _swrit.ToString().Should().Be("<2> 10 5");
        }

        [TestMethod]
        public void TestEmit()
        {
            Interpret("48 char D emit emit");
            _swrit.ToString().Should().Be("D0");
        }

        [TestMethod]
        public void TestCr()
        {
            Interpret("cr");
            _swrit.ToString().Should().Be(Environment.NewLine);
        }

        [TestMethod]
        public void TestPage()
        {
            Interpret("page");
            _swrit.ToString()[0].Should().Be((char)12);
        }

        [TestMethod]
        public void TestType()
        {
            Interpret("S\" Hello World\" type");
            _swrit.ToString().Should().Be("Hello World");
        }

		private void Interpret(string script)
        {
            var intrp = new Interpreter();
            intrp.Interpret(script);
        }

        private void Reset()
        {
            StringBuilder sb = _swrit.GetStringBuilder();
            sb.Remove(0, sb.Length);
		}
	}
}
