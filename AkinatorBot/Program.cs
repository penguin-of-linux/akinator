using System;
using System.Reflection;
using System.Threading.Tasks;
using GroboContainer.Core;
using GroboContainer.Impl;
using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace AkinatorBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var token = "";
            var proxy = new HttpToSocks5Proxy("orbtl.s5.opennetwork.cc", 999, "226624896", "1ZXug6vY");

            var container = InitializeContainer();
            akinator = container.Get<IAkinator>();

            botClient = new TelegramBotClient(token, proxy);
            botClient.OnMessage += OnMessage;
            botClient.StartReceiving();

            Task.Delay(-1).Wait();
        }

        static IContainer InitializeContainer()
        {
            var container = new Container(new ContainerConfiguration(Assembly.GetExecutingAssembly()));
            return container;
        }

        static async void OnMessage(object sender, MessageEventArgs e)
        {
            await botClient.SendTextMessageAsync(
                chatId: e.Message.Chat,
                text: "You said:\n" + e.Message.Text
            );
        }

        private static TelegramBotClient botClient;
        private static IAkinator akinator;
    }
}
