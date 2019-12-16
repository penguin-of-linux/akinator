using System.Linq;

namespace AkinatorBot.Commands
{
    public class AddCommand : IBotCommand
    {
        public AkinatorAnswer Invoke(IAkinator akinator, string input)
        {
            var name = "";
            var words = input.Split().Skip(1).ToArray();
            for (var i = 0; i < words.Length - 1; i++)
            {
                name += words[i] + " ";
            }
            name += words.Last();

            akinator.AddCharacter(name);
            return null;
        }

        public bool Validate(string input)
        {
            return input.StartsWith("add");
        }
    }
}