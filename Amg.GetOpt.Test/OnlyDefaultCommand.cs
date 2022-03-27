using System;
using System.ComponentModel;

namespace Amg.GetOpt.Test;

internal class OnlyDefaultCommand
{
    [Description("Greets the world.")]
    public void Greet()
    {
        Console.WriteLine("Hello, world.");
    }
}
