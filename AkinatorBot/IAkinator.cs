namespace AkinatorBot
{
    public interface IAkinator
    {
        AkinatorAnswer Start();
        AkinatorAnswer NextQuestion(UserAnswer userAnswer);
    }
}