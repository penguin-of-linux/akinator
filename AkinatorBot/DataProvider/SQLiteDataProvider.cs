using System.Collections.Generic;
using System.IO;
using System.Linq;
using AkinatorBot.DatabaseContexts;
using Newtonsoft.Json;

namespace AkinatorBot.DataProvider
{
    public class SQLiteDataProvider: IDataProvider
    {
        private readonly string[] questions;

        public SQLiteDataProvider()
        {
            questions = JsonConvert.DeserializeObject<string[]>(File.ReadAllText("files/questions.json"));
            
            using (var db = new CharacterEntryDatabaseContext())
            {
                db.Database.EnsureCreated();
            }
        }

        public string[] GetQuestions()
        {
            return questions;
        }

        public CharacterEntry[] GetCharacters()
        {
            List<CharacterEntry> characters = new List<CharacterEntry>();

            using (var db = new CharacterEntryDatabaseContext())
            {
                foreach (var character in db.CharacterEntries.ToList())
                {
                    characters.Add(character);
                }
            }

            return characters.ToArray();
        }

        public void Save(IEnumerable<CharacterEntry> characters)
        {
            using (var db = new CharacterEntryDatabaseContext())
            {
                foreach (var character in characters)
                {
                    db.CharacterEntries.Add(character);
                }

                db.SaveChanges();
            }
        }

        public int GetGameCount()
        {
            int count = 0;

            using (var db = new CharacterEntryDatabaseContext())
            {
                count = db.CharacterEntries.Count();
            }

            return count;
        }

        public void SaveGameCount(int count)
        {
            throw new System.NotImplementedException();
        }
    }
}