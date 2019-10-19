namespace Amg.GetOpt
{
    public interface ICommandProvider
    {
        ICommand GetCommand(string name);
        IOption GetLongOption(string optionName);
        IOption GetShortOption(string optionName);
    }
}