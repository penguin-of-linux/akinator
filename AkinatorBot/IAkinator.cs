namespace AkinatorBot
{
    public interface IAkinator
    {
        AkinatorAnswer Start();
        AkinatorAnswer NextQuestion(UserAnswer userAnswer);
        AkinatorAnswer Suppose();
        void AddCharacter(string name);
        void AddCharacter(CharacterEntry character);
        void Save();
    }
}