using System.Collections.Generic;

namespace AkinatorBot.DataProvider
{
    public interface IDataProvider
    {
        string[] GetQuestions();
        CharacterEntry[] GetCharacters();
        void Save(IEnumerable<CharacterEntry> characters);
        int GetGameCount();
        void SaveGameCount(int count);
    }
}