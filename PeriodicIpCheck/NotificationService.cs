using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Text;

namespace PeriodicIpCheck {
    internal class NotificationService {
        private readonly Config _config;
        public NotificationService(Config config) {
            _config = config;
        }

        public void SendNotification(IpRecord currentIp, IpRecord? lastIp) {
            if (!_config.NotificationsConfigured) return;
            string emailBody = GetEmailBody(currentIp, lastIp);
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_config.SenderEmail!.Split("@").First(), _config.SenderEmail));
            message.To.Add(new MailboxAddress(_config.NotificationEmail!.Split("@").First(), _config.NotificationEmail));
            message.Subject = "Your IP has changed.";
            message.Body = new TextPart("plain") {
                Text = emailBody
            };

            using (var client = new SmtpClient()) {
                client.Connect(_config.SmtpServer, _config.SmtpPort, SecureSocketOptions.StartTls);
                client.Authenticate(_config.SenderEmail, _config.SenderPassword);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        private string GetEmailBody(IpRecord currentIp, IpRecord? lastIp) {
            var sb = new StringBuilder();
            sb.AppendLine("IP has changed!");
            sb.AppendLine("Current IP:");
            sb.AppendLine("\t" + currentIp.ToString(false));
            if (lastIp != null) {
                sb.AppendLine("Previous IP:");
                sb.AppendLine("\t" + lastIp.ToString(false));
            }
            return sb.ToString();
        }
    }
}
