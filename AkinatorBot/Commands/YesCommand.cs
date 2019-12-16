namespace AkinatorBot.Commands
{
    public class YesCommand : IBotCommand
    {
        public AkinatorAnswer Invoke(IAkinator akinator, string input)
        {
            return akinator.NextQuestion(UserAnswer.Yes);
        }

        public bool Validate(string input)
        {
            return input == "yes";
        }
    }
}