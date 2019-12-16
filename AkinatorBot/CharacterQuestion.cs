using System;

namespace AkinatorBot
{
    public class CharacterQuestion
    {
        public int Id { get; set; }
        public int Count { get; set; }

        public double Probability
        {
            get => _probability;
            set => _probability = Math.Max(value, 0.1);
        }

        private double _probability;
    }
}