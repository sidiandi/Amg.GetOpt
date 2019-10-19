using NUnit.Framework;

namespace Amg.GetOpt.Test
{
    [TestFixture]
    class GetOptTests
    {
        [Test]
        public void Run()
        {
            var co = new TestCommandObject();
            var exitCode = GetOpt.Run(new[]
            {
                "add",
                "1",
                "--name",
                "Alice",
                "2",
                "subtract",
                "3",
                "2",
                "greet"
            }, co);
            Assert.That(exitCode, Is.EqualTo(0));
            Assert.That(co.Name, Is.EqualTo("Alice"));
        }

        [Test]
        public void ArgumentMissing()
        {
            var co = new TestCommandObject();
            var exitCode = GetOpt.Run(new[]
            {
                "add",
                "1",
            }, co);
            Assert.That(exitCode, Is.Not.EqualTo(0));
        }
    }
}
