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
                TelegramBotClass tgc = new TelegramBotClass(token: "6925471651:AAFmdQf0Ew6zSEeU92BOUWHw5GNhfu9zNE0");
                tgc.GetUpdates();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}