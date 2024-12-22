using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Здесь будет логика отправки письма.
        // Сейчас просто выводим в консоль для теста.
        Console.WriteLine($"Email: {email}, Subject: {subject}, Message: {htmlMessage}");
        return Task.CompletedTask;
    }
}
