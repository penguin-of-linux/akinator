namespace AkinatorBot.Commands
{
    public interface IBotCommand
    {
        AkinatorAnswer Invoke(IAkinator akinator, string input);
        bool Validate(string input);
    }
}