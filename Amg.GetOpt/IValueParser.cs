using System;

namespace Amg.GetOpt
{
    interface IValueParser
    {
        public bool TryParse(ParserState args, Type toType, out object? value);
    }
}
