using System.Collections.Generic;

namespace AkinatorBot
{
    public class CharacterEntry
    {
        public string Name { get; set; }
        public List<int> Questions { get; set; }
        public double StartProbability { get; set; }
    }
}