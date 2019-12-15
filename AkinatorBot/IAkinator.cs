namespace AkinatorBot
{
    public interface IAkinator
    {
        AkinatorAnswer Start();
        AkinatorAnswer NextQuestion(UserAnswer userAnswer);
        void AddCharacter(string name);
        void AddCharacter(CharacterEntry character);
        void Save();
    }
}