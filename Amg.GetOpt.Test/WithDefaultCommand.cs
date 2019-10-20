using System.ComponentModel;

namespace Amg.GetOpt.Test
{
    internal class WithDefaultCommand
    {
        public WithDefaultCommand()
        {
        }

        [Default, Description("do something")]
        public void DoSomething()
        {
            done = true;
        }

        public bool done = false;
    }
}