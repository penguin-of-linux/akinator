namespace AkinatorBot.Commands
{
    public class SaveCommand : IBotCommand
    {
        public AkinatorAnswer Invoke(IAkinator akinator, string input)
        {
            akinator.Save();
            return null;
        }

        public bool Validate(string input)
        {
            return input == "save";
        }
    }
}