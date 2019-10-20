using System.Collections.Generic;

namespace Amg.GetOpt
{
    public interface ICommandProvider
    {
        IEnumerable<IOption> Options();
        IEnumerable<ICommand> Commands();
    }

}