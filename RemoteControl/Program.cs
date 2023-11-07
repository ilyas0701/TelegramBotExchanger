using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bots.Extensions.Polling;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bots.Types;
using Update = Telegram.Bot.Types.Update;

namespace TelegramBotExperiments
{

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TelegramBotClass tgc = new TelegramBotClass(token: "TOKEN");
                tgc.GetUpdates();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}