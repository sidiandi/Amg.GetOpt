using System;
using System.ComponentModel;

namespace example
{
    class Program
    {
        static void Main(string[] args) => Amg.GetOpt.GetOpt.Run(args, new Program());

        [Description("Add two numbers.")]
        public int Add(int a, int b)
        {
            return a + b;
        }

        [Description("Greet the world.")]
        public void Greet()
        {
            Console.WriteLine("Hello, {Name}.");
        }

        [Description("Name to greet")]
        public string Name { get; set; } = "world";
    }
}
