namespace AkinatorBot.Commands
{
    public class StartCommand : IBotCommand
    {
        public AkinatorAnswer Invoke(IAkinator akinator, string input)
        {
            return akinator.Start();
        }

        public bool Validate(string input)
        {
            return input == "start";
        }
    }
}