namespace AkinatorBot
{
    public class CharacterEntry
    {
        public string Name { get; set; }
        public CharacterQuestion[] Questions { get; set; }
        public int Count { get; set; }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is CharacterEntry entry)
            {
                return entry.Name == Name;
            }

            return false;
        }
    }
}