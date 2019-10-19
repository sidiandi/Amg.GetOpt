using System.ComponentModel;

namespace Amg.GetOpt.Test
{
    [Description("Test command object")]
    internal class TestCommandObject
    {
        [Description("add two numbers")]
        public int Add(int a, int b)
        {
            return a + b;
        }

        [Description("subtract two numbers")]
        public int Subtract(int a, int b)
        {
            return a - b;
        }

        [Description("name to be greeted")]
        public string? Name { get; set; }

        [Short("h"), Description("Show help")]
        public bool Help { get; set; }

        [Description("Option with long name")]
        public string? LongOption { get; set; }

        [Short("v"), Description("Enum option")]
        public Verbosity Verbosity { get; set; }
    }

    enum Verbosity
    { 
        Quiet,
        Error,
        Detailed
    }

}