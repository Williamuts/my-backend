namespace E1.Backend.Api.Services
{
    public class MockEmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            Console.WriteLine($"======== MOCK EMAIL ========");
            Console.WriteLine($"To: {toEmail}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {body}");
            Console.WriteLine($"============================");
            await Task.CompletedTask;
        }
    }
}