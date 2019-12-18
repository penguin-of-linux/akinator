using Microsoft.EntityFrameworkCore;

namespace AkinatorBot.DatabaseContexts
{
    public class CharacterEntryDatabaseContext: DbContext
    {
        public DbSet<CharacterEntry> CharacterEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            // Specify the path of the database here
            optionsBuilder.UseSqlite("Filename=./characters.sqlite");
        }
    }
}