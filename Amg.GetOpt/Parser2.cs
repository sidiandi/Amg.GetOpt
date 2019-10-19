using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Amg.GetOpt
{
    class Parser2
    {
        private readonly CommandProvider commandProvider;
        private readonly ValueParser valueParser = new ValueParser();

        ParserState state;
        bool handleArgs = false;

        readonly List<ParserState> operands = new List<ParserState>();

        public IEnumerable<string> Operands => operands.Select(_ => _.Current);

        public Parser2(CommandProvider commandProvider)
        {
            this.commandProvider = commandProvider;
            this.state = new ParserState(new string[] { });
        }

        public void Parse(string[] args)
        {
            this.state = new ParserState(args);
            handleArgs = true;
            operands.Clear();

            while (state.HasCurrent)
            {
                if (OptionStop())
                {
                    // nothing
                }
                else if (LongOption())
                {
                    // nothing
                }
                else if (ShortOption())
                {
                    // nothing
                }
                else
                {
                    Operand();
                }
            }
        }

        private void Operand()
        {
            operands.Add(state.Clone());
            state.Consume();
        }

        private bool ShortOption()
        {
            var p = Current.Split(new[] { shortOptionPrefix }, 2, StringSplitOptions.None);
            if (p.Length > 1 && handleArgs)
            {
                state.Consume();
                string? optionText = p[1];

                while (optionText != null)
                {
                    var keyValue = SplitFirstCharacter(optionText);
                    var name = keyValue[0];
                    var value = keyValue.Length > 1 ? keyValue[1] : null;
                    var option = commandProvider.ShortGetOption(name);
                    option.Set(ref value, state, valueParser);
                    optionText = value;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        static string[] SplitFirstCharacter(string x)
        {
            if (x.Length == 0)
            {
                return new string[] { };
            }
            else if (x.Length == 1)
            {
                return new string[] { x };
            }
            else
            {
                return new string[] { x.Substring(0, 1), x.Substring(1) };
            }
        }

        private bool LongOption()
        {
            var p = Current.Split(new[] { longOptionPrefix }, 2, StringSplitOptions.None);
            if (p.Length > 1 && handleArgs)
            {
                state.Consume();
                var keyValue = p[1].Split(new[] { "=" }, 2, StringSplitOptions.None);
                var name = keyValue[0];
                var value = keyValue.Length > 1 ? keyValue[1] : null;

                var option = commandProvider.LongGetOption(name);
                option.Set(value, state, valueParser);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool OptionStop()
        {
            if (Current.Equals(longOptionPrefix))
            {
                state.Consume();
                handleArgs = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        string Current => state.Current;

        const string longOptionPrefix = "--";
        const string shortOptionPrefix = "-";

        public static string LongNameForCsharpIdentifier(string memberName)
        {
            return new string(memberName.Take(1)
                .Select(char.ToLower)
                .Concat(memberName.Skip(1)
                    .SelectMany(_ => char.IsUpper(_)
                        ? new[]
                        {
                            '-',
                            char.ToLower(_)
                        }
                        : new[]
                        {
                            _
                        }))
                .ToArray());
        }

        public static string LongName(PropertyInfo p)
            => LongNameForCsharpIdentifier(p.Name);

        public static string LongName(MethodInfo m)
            => LongNameForCsharpIdentifier(m.Name);
    }
}
