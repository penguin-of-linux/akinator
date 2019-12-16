namespace AkinatorBot.Commands
{
    public class NoCommand : IBotCommand
    {
        public AkinatorAnswer Invoke(IAkinator akinator, string input)
        {
            return akinator.NextQuestion(UserAnswer.No);
        }

        public bool Validate(string input)
        {
            return input == "no";
        }
    }
}