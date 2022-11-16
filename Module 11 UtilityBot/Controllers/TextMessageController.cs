using Module_11_UtilityBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Module_11_UtilityBot.Controllers
{
    public class TextMessageController
    {
        private readonly ITelegramBotClient _telegramClient;
        private readonly IStorage _memoryStorage;

        public TextMessageController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {
            switch (message.Text)
            {
                case "/start":

                    // Объект, представляющий кноки
                    var buttons = new List<InlineKeyboardButton[]>();
                    buttons.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData($" Длина сообщения" , $"length"),
                        InlineKeyboardButton.WithCallbackData($" Сумма чисел" , $"sum")
                    });

                    // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"<b>  Наш бот считает сумму чисел или длину сообщения.</b> {Environment.NewLine}" +
                        $"{Environment.NewLine}Выберите режим работы.{Environment.NewLine}", cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));

                    break;
                default:
                    bool sumMode = _memoryStorage.GetSession(message.Chat.Id).SumMode;

                    if (sumMode == false)
                    {
                        await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"В вашем сообщении {message.Text.Length} символов", cancellationToken: ct);
                    }
                    else
                    {
                        int result = 0;
                        try
                        {
                            string[] words = message.Text.Split(' ');
                            
                            foreach (var word in words)
                            {
                                result += Int32.Parse(word);
                            }
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Ошибка форматирования - введите числа через пробел", cancellationToken: ct);
                            return;
                        }

                        await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Сумма чисел: {result}", cancellationToken: ct);
                    }
                    break;
            }
        }
    }
}
