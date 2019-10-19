using Amg.Extensions;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Amg.GetOpt
{
    static class Help
    {
        public static void PrintHelpMessage(TextWriter w, ICommandProvider commandProvider)
        {
            var c = commandProvider.Commands().OrderBy(_ => _.Name);

            var name = Assembly.GetEntryAssembly().GetName().Name;

            w.WriteLine($"Usage: {name} [options] [command] [arguments]...");
            if (c.Any())
            {
                w.WriteLine();
                w.WriteLine("Commands:");
                c.Select(_ => new { _.Syntax, _.Description }).ToTable().Write(w);
            }

            var o = commandProvider.Options().OrderBy(_ => _.Long);
            if (o.Any())
            {
                w.WriteLine();
                w.WriteLine("Options:");
                o.Select(_ => new { _.Syntax, _.Description })
                    .ToTable().Write(w);
            }
        }
    }
}
