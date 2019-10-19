using System;

namespace Amg.GetOpt
{
    public interface IValueParser
    {
        public bool TryParse(ParserState args, Type toType, out object? value);
    }
}
