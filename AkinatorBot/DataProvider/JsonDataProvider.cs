using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace AkinatorBot.DataProvider
{
    public class JsonDataProvider : IDataProvider
    {
        public JsonDataProvider()
        {
            questions = JsonConvert.DeserializeObject<string[]>(File.ReadAllText("files/questions.json"));
            characters = JsonConvert.DeserializeObject<CharacterEntry[]>(File.ReadAllText("files/characters.json"));
        }
        
        public string[] GetQuestions()  
        {
            return questions;
        }

        public CharacterEntry[] GetCharacters()
        {
            return characters;
        }

        private readonly string[] questions;
        private readonly CharacterEntry[] characters;
    }
}