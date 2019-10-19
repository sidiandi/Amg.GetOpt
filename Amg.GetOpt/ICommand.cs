using System.Threading.Tasks;

namespace Amg.GetOpt
{
    public interface ICommand
    {
        string Name { get; }
        Task<object?> Invoke(ParserState args, IValueParser valueParser);
        string Syntax { get; }
        string Description { get; }
    }
}