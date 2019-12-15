using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GroboContainer.Core;
using GroboContainer.Impl;
using MihaZupan;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace AkinatorBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxy = new HttpToSocks5Proxy("orbtl.s5.opennetwork.cc", 999, "226624896", "1ZXug6vY");

            var container = InitializeContainer();
            akinator = container.Get<IAkinator>();

            //botClient = new TelegramBotClient(token, proxy);
            botClient = new TelegramBotClient(token);
            botClient.OnMessage += OnMessage;
            botClient.StartReceiving();

            Task.Delay(-1).Wait();
        }

        static IContainer InitializeContainer()
        {
            var container = new Container(new ContainerConfiguration(Assembly.GetExecutingAssembly()));
            return container;
        }

        static void OnMessage(object sender, MessageEventArgs e)
        {
            AkinatorAnswer answer;
            if(e.Message.Text.StartsWith("start"))
            {
                started = true;
                answer = akinator.Start();
            }
            else if(!started)
            {
                return;
            }
            else if(e.Message.Text.StartsWith("yes"))
            {
                answer = akinator.NextQuestion(UserAnswer.Yes);
            }
            else if(e.Message.Text.StartsWith("no"))
            {
                answer = akinator.NextQuestion(UserAnswer.No);
            }
            else if(e.Message.Text.StartsWith("idk"))
            {
                answer = akinator.NextQuestion(UserAnswer.DontKnow);
            }
            else if(e.Message.Text.StartsWith("add"))
            {
                var name = "";
                var words = e.Message.Text.Split().Skip(1).ToArray();
                for (var i = 0; i < words.Length - 1; i++)
                {
                    name += words[i] + " ";
                }
                name += words.Last();
                
                akinator.AddCharacter(name);
                return;
            }
            else if(e.Message.Text.StartsWith("save"))
            {
                akinator.Save();
                return;
            }
            else answer = new AkinatorAnswer
            {
                Message = "Pls, type 'start', 'yes', 'no' or 'idk'"
            };

            var message = AnswerToString(answer);
            //var shitEncoding = new UTF8Encoding(false);
            //var bytes = Encoding.UTF8.GetBytes(message);
            //var converted = Encoding.Convert(Encoding.UTF8, shitEncoding, bytes);
            botClient.SendTextMessageAsync(e.Message.Chat.Id, message);
        }

        static string AnswerToString(AkinatorAnswer answer)
        {
            return answer.Message;
        }

        private static bool started = false;
        private static TelegramBotClient botClient;
        private static IAkinator akinator;
    }
}
