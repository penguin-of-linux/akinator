namespace AkinatorBot.Commands
{
    public class IdkCommand : IBotCommand
    {
        public AkinatorAnswer Invoke(IAkinator akinator, string input)
        {
            return akinator.NextQuestion(UserAnswer.DontKnow);
        }

        public bool Validate(string input)
        {
            return input == "idk";
        }
    }
}