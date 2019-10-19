using System;

namespace Amg.GetOpt
{
    sealed class ParserState
    {
        private readonly ArraySegment<string> args;
        private int pos;

        public ParserState(string[] args)
        {
            this.args = new ArraySegment<string>(args);
            this.pos = 0;
        }

        public ParserState(ParserState other)
        {
            args = other.args;
            pos = other.pos;
        }

        public ParserState Clone()
        {
            return new ParserState(this);
        }

        internal void SetPos(ParserState temp)
        {
            if (temp.args != this.args)
            {
                throw new ArgumentException("not the same array");
            }
            pos = temp.pos;
        }

        public string Current => args.Array[pos+args.Offset];

        public bool HasCurrent => pos < args.Count;

        public string Consume()
        {
            if (HasCurrent)
            {
                var c = Current;
                ++pos;
                return c;
            }
            else
            {
                throw new InvalidOperationException("no more elements");
            }
        }

        public void Reset()
        {
            pos = 0;
        }
    }
}
