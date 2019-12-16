namespace AkinatorBot.Commands
{
    public class SupposeCommand : IBotCommand
    {
        public AkinatorAnswer Invoke(IAkinator akinator, string input)
        {
            return akinator.Suppose();
        }

        public bool Validate(string input)
        {
            return input == "suppose";
        }
    }
}