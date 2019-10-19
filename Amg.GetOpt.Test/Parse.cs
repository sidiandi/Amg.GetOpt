using NUnit.Framework;
using System.Linq;

namespace Amg.GetOpt.Test
{
    public class ParseTests : TestBase
    {
        [Test]
        public void Parse()
        {
            var args = new[] { "--name", "Alice", "-h", "--long-option=value", "hello", "-vdetailed", "--", "--not-an-option" };
            var o = new TestCommandObject();
            var parser = new Parser(new CommandProvider(o));
            parser.Parse(args);
            Assert.That(true);
        }

        [Test]
        public void Parse2()
        {
            var args = new[] { "add", "1", "--name", "Alice", "2", "subtract", "3", "2" };
            var o = new TestCommandObject();
            var parser = new Parser(new CommandProvider(o));
            parser.Parse(args);
            Assert.That(parser.Operands.Count() == 6);
        }
    }
}