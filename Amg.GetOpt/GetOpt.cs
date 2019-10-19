using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Amg.GetOpt.Test")]

namespace Amg.GetOpt
{
    public static class GetOpt
    {
        public static int Run(string[] args, object commandObject)
        {
            var p = new Parser(new CommandProvider(commandObject));
            p.Parse(args);
            try
            {
                var result = p.Run().Result;
                foreach (var i in result)
                {
                    Console.WriteLine(i);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return -1;
            }
            return 0;
        }
    }
}
