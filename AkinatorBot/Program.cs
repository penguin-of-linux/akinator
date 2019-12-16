using System.Reflection;
using System.Threading.Tasks;
using AkinatorBot.Commands;
using GroboContainer.Core;
using GroboContainer.Impl;
using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace AkinatorBot
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var proxy = new HttpToSocks5Proxy("orbtl.s5.opennetwork.cc", 999, "226624896", "1ZXug6vY");

            var container = InitializeContainer();
            _akinator = container.Get<IAkinator>();
            _commands = container.GetAll<IBotCommand>();
            _botClient = new TelegramBotClient(token, proxy);
            //botClient = new TelegramBotClient(token);
            _botClient.OnMessage += OnMessage;
            _botClient.StartReceiving();

            Task.Delay(-1).Wait();
        }

        private static IContainer InitializeContainer()
        {
            var container = new Container(new ContainerConfiguration(Assembly.GetExecutingAssembly()));
            return container;
        }

        private static void OnMessage(object sender, MessageEventArgs e)
        {
            if (!GetAnswer(e.Message.Text, out var answer))
                answer = new AkinatorAnswer
                {
                    Message = "Pls, type 'start', 'yes', 'no' or 'idk'"
                };

            if (answer == null) return;
            var message = AnswerToString(answer);
            _botClient.SendTextMessageAsync(e.Message.Chat.Id, message);
        }

        private static bool GetAnswer(string input, out AkinatorAnswer answer)
        {
            answer = null;
            foreach (var command in _commands)
                if (command.Validate(input))
                {
                    answer = command.Invoke(_akinator, input);
                    return true;
                }

            return false;
        }

        private static string AnswerToString(AkinatorAnswer answer)
        {
            return answer.Message;
        }

        private static IBotCommand[] _commands;
        private static TelegramBotClient _botClient;
        private static IAkinator _akinator;
    }
}