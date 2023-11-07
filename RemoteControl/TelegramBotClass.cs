using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RemoteControl;
using System.Net;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bots.Types;
using Update = Telegram.Bot.Types.Update;

namespace TelegramBotExperiments
{
    internal class TelegramBotClass
    {
        private string _token;
        private Telegram.Bot.TelegramBotClient _client;

        private const string _buttonToAdd = "Перевод долларів в гривні";
        private const string _cheakButtonToAdded = "Подивитись курс доллара для гривні";

        public TelegramBotClass(string token)
        {
            this._token = token;
        }

        internal void GetUpdates()
        {
            _client = new Telegram.Bot.TelegramBotClient(_token);
            var me = _client.GetMeAsync().Result;
            if (me != null || !string.IsNullOrEmpty(me.Username))
            {
                int? offset = 0;
                while (true)
                {
                    try
                    {
                        var updates = _client.GetUpdatesAsync(offset).Result;
                        if (updates != null && updates.Count() > 0)
                        {
                            foreach (var update in updates)
                            {
                                ProcessUpdateAsync(update);
                                offset = update.Id + 1;
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    Thread.Sleep(1000);
                }
            }
        }
        private bool isWaitingForInput = false;

        private async Task ProcessUpdateAsync(Telegram.Bot.Types.Update update)
        {
            switch (update.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    var text = update.Message.Text;
                    var chatId = update.Message.Chat.Id;
                    var sale = await GetMoneySale();

                    if (text.StartsWith("/start"))
                    {
                        string welcomeMessage = "Привіт! Ласкаво просимо! Цей робот створений щоб дізнаватися про поточний курс валют і за потребою переводити долари ви гривні. Приємного користування)";
                        await _client.SendTextMessageAsync(chatId, welcomeMessage, replyMarkup: GetButtonsMain());
                    }
                    else if (isWaitingForInput)
                    {
                        if (double.TryParse(text, out double userInput))
                        {
                            var result = userInput * (double)sale;
                            await _client.SendTextMessageAsync(update.Message.Chat.Id, $"{userInput.ToString("F2")} $ це {result.ToString("F2")} грн.", replyMarkup: GetButtonsMain());
                        }
                        else
                        {
                            await _client.SendTextMessageAsync(update.Message.Chat.Id, "Будь ласка, введіть число", replyMarkup: GetButtonsMain());
                        }
                        isWaitingForInput = false;
                    }
                    else
                    {
                        switch (text)
                        {
                            case _buttonToAdd:
                                isWaitingForInput = true;
                                await _client.SendTextMessageAsync(update.Message.Chat.Id, "Введіть кількість $:", replyMarkup: new ReplyKeyboardRemove());
                                break;
                            case _cheakButtonToAdded:
                                string moneyInfo = await GetMoneyInfo();
                                await _client.SendTextMessageAsync(update.Message.Chat.Id, moneyInfo, replyMarkup: GetButtonsMain());
                                break;
                            default:
                                await _client.SendTextMessageAsync(update.Message.Chat.Id, "Опсс, такої команди ще нема", replyMarkup: GetButtonsMain());
                                break;
                        }
                    }
                    break;
                default:
                    Console.WriteLine(update.Type);
                    break;
            }
        }

        private IReplyMarkup? GetButtonsMain()
        {
            var keyboardButtons = new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>
                {
                    new KeyboardButton("  " + _buttonToAdd + "  "),
                    new KeyboardButton("  " + _cheakButtonToAdded + "  ")
                }
            };

            return new ReplyKeyboardMarkup(keyboardButtons);
        }

        private async Task<decimal> GetMoneyBuy()
        {
            CurrencyRateViewModel viewModel = new CurrencyRateViewModel();
            await viewModel.LoadData();

            return viewModel.Buy;
        }

        private async Task<decimal> GetMoneySale()
        {
            CurrencyRateViewModel viewModel = new CurrencyRateViewModel();
            await viewModel.LoadData();

            return viewModel.Sale;
        }

        private async Task<string> GetMoneyInfo()
        {
            var moneyBuy = await GetMoneyBuy();
            var moneySale = await GetMoneySale();

            return $"Покупка = {moneyBuy.ToString("F2")} грн. Продажа = {moneySale.ToString("F2")} грн.";
        }
    }
}