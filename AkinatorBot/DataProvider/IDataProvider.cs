namespace AkinatorBot.DataProvider
{
    public interface IDataProvider
    {
        string[] GetQuestions();
        CharacterEntry[] GetCharacters();
    }
}