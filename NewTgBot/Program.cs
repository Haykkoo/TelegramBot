using ApiAiSDK;
using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace NewTgBot
{
    class Program
    {
        private static TelegramBotClient Bot;
        private static ApiAi apiAi;
        private static void Main(string[] args)
        {
            //Bot Token
            Bot = new TelegramBotClient("################################");
            AIConfiguration config = new AIConfiguration("#######################", SupportedLanguage.English);
            apiAi = new ApiAi(config);


            var me = Bot.GetMeAsync().Result;
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnCallbackQuery += Bot_OnCallbackQueryReceived;

            Console.WriteLine(me.FirstName);

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void Bot_OnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            string buttonText = e.CallbackQuery.Data;
            string name = $"{e.CallbackQuery.From.FirstName}, {e.CallbackQuery.From.LastName}";
            Console.WriteLine($"{name} press {buttonText}");
            await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"You preess{buttonText}");
            if (buttonText == "Picture")
            {
                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "https://godliteratury.ru/wp-content/uploads/2019/08/12-luchshikh-rasskazov-Borkhesa.jpg");
            }
            else if (buttonText == "Video")
            {
                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "https://www.youtube.com/watch?v=mJeLGd3JV2I");
            }
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            string name = $"{message.From.FirstName} {message.From.LastName}";
            Console.WriteLine($"{ name} send your message : {message.Text}");

            switch (message.Text)
            {
                case "/start":
                    string text = "/start  - Start\n" +
                                  "/inline  - Menu\n" +
                                  "/keyboard  - Keyboard";


                    await Bot.SendTextMessageAsync(message.From.Id, text);
                    break;

                case "/inline":
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("Բորխես","http://grapaharan.org/%D4%BD%D5%B8%D6%80%D5%AD%D5%A5_%D4%BC%D5%B8%D6%82%D5%AB%D5%BD_%D4%B2%D5%B8%D6%80%D5%AD%D5%A5%D5%BD"),
                            InlineKeyboardButton.WithUrl("Նախաբանի փոխարեն", "http://grapaharan.org/%C2%AB%D4%B1%D6%80%D5%A1%D6%80%D5%AB%D5%B9%D5%A8%C2%BB_%D5%A3%D6%80%D6%84%D5%AB_%D5%B6%D5%A1%D5%AD%D5%A1%D5%A2%D5%A1%D5%B6%D5%A8")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Picture" ),
                            InlineKeyboardButton.WithCallbackData("Video" )
                        }
                    });

                    await Bot.SendTextMessageAsync(message.From.Id, "Choose Menu ", replyMarkup: inlineKeyboard);

                    break;

                case "/keyboard":
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Ողջույն"),
                            new KeyboardButton("այստե՞ղ ես")

                        },
                        new[]
                        {
                            new KeyboardButton("Conntact"){RequestContact=true },
                            new KeyboardButton("Location"){RequestLocation=true}

                        }
                    });
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Message :", replyMarkup: replyKeyboard);
                    break;

                default:
                    var response = apiAi.TextRequest(message.Text);
                    string answer = response.Result.Fulfillment.Speech;
                    if (answer == "")
                    {
                        answer = "Sorry i cant understand you";
                    }

                    await Bot.SendTextMessageAsync(message.Chat.Id, answer);

                    break;
            }
        }
    }
}