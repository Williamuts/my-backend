using MimeKit;
// 👇 关键修改：明确指定 SmtpClient 是 MailKit 的，防止和系统自带的冲突
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace E1.Backend.Api.Services
{
    public class GmailEmailService : IEmailService
    {
        // 配置信息 (建议以后移到 appsettings.json，现在先写死以便测试)
        private const string SmtpServer = "smtp.gmail.com";
        private const int SmtpPort = 587;

        // 👇👇👇 请在这里填入你申请到的 Gmail 信息 👇👇👇
        private const string SenderName = "E1 App";
        private const string SenderEmail = "你的邮箱@gmail.com";
        private const string SenderPassword = "你的16位应用专用密码";

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(SenderName, SenderEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            // 这里使用纯文本发送
            message.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                try
                {
                    // 1. 连接 Gmail 服务器
                    await client.ConnectAsync(SmtpServer, SmtpPort, false);

                    // 2. 认证 (使用应用专用密码)
                    await client.AuthenticateAsync(SenderEmail, SenderPassword);

                    // 3. 发送邮件
                    await client.SendAsync(message);

                    // 4. 断开连接
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    // 如果发送失败，打印错误信息
                    Console.WriteLine($"邮件发送失败: {ex.Message}");
                    throw; // 继续抛出异常，让 Controller 知道发送失败了
                }
            }
        }
    }
}