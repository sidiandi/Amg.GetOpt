using Amg.GetOpt;
using System;

namespace example;

class Program
{
    static void Main(string[] args) => Amg.GetOpt.GetOpt.Run(args, new Program());

    public int Add(int a, int b)
    {
        return a + b;
    }

    public void Greet()
    {
        Console.WriteLine($"Hello, {Name}.");
    }

    public string Name { get; set; } = "world";
}
