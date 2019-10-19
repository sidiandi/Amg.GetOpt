using System;
using System.Collections.Generic;

namespace Amg.GetOpt.Tokens
{
    public class Token
    {
        protected Token()
        {

        }

        public static IEnumerable<Token> Tokenize(IEnumerable<string> args)
        {
            bool argsOnly = false;
            foreach (var i in args)
            {
                if (argsOnly)
                {
                    yield return new Arg(i);
                }
                else
                {
                    if (i.Equals(LongOption.prefix))
                    {
                        argsOnly = true;
                    }
                    else if (LongOption.Is(i))
                    {
                        yield return new LongOption(i);
                    }
                    else if (ShortOption.Is(i))
                    {
                        yield return new ShortOption(i);
                    }
                    else
                    {
                        yield return new Arg(i);
                    }
                }
            }
        }
    }

    public class Arg : Token
    {
        public Arg(string arg)
        {
            Value = arg;
        }
        public string Value { get; }

        public override string ToString()
        {
            return Value;
        }
    }

    public class LongOption : Token
    {
        internal const string prefix = "--";
        public LongOption(string arg)
        {
            var p = arg.Split(new[] { prefix }, StringSplitOptions.None);
            var nameValue = p[1].Split(new[] { "=" }, 2, StringSplitOptions.None);
            Name = nameValue[0];
            Value = nameValue.Length > 1
                ? nameValue[1]
                : null;
        }

        public static bool Is(string arg)
        {
            return arg.StartsWith(prefix);
        }

        public string Name { get; }
        public string? Value { get; }
    }

    public class ShortOption : Token
    {
        const string prefix = "-";
        public ShortOption(string arg)
        {
            var p = arg.Split(new[] { prefix }, StringSplitOptions.None);
            Name = p[1].Substring(0, 1);
            Value = p[1].Length > 1
                ? p[1].Substring(1)
                : null;
        }

        public static bool Is(string arg)
        {
            return arg.StartsWith(prefix);
        }

        public string Name { get; }
        public string? Value { get; }
    }
}
