using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PeriodicIpCheck {
    internal class Config {
        public string ResultsFilePath { get; set; } = "ip_stats.txt";
        public string LogFilePath { get; set; } = "ip_stats.log";
        public bool LogOnlyChanges { get; set; } = true;
        public string? NotificationEmail { get; set; } = null;
        public string? SmtpServer { get; set; } = null;
        public int SmtpPort { get; set; } = 587;
        public string? SenderEmail { get; set; } = null;
        public string? SenderPassword { get; set; } = null;
        [JsonIgnore]
        public bool IsDefault { get; private set; } = true;
        [JsonIgnore]
        public bool NotificationsConfigured => !string.IsNullOrWhiteSpace(NotificationEmail) &&
                                               !string.IsNullOrWhiteSpace(SmtpServer) &&
                                               !string.IsNullOrWhiteSpace(SenderEmail) &&
                                               !string.IsNullOrWhiteSpace(SenderPassword);

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine("Config:");
            sb.AppendLine($"\tResults file: {ResultsFilePath}");
            sb.AppendLine($"\tLog file: {LogFilePath}");
            sb.AppendLine($"\tLog only changes: {LogOnlyChanges}");
            sb.AppendLine($"\tNotification email: {NotificationEmail}");
            sb.AppendLine($"\tSMTP server: {SmtpServer}");
            sb.AppendLine($"\tSender email: {SenderEmail}");
            if(!string.IsNullOrWhiteSpace(SenderPassword)) {
                sb.AppendLine("\tSender password set.");
            }else {
                sb.AppendLine("\tSender password NOT set.");
            }
            if (NotificationsConfigured) {
                sb.AppendLine("\tNotifications are enabled.");
            }
            return sb.ToString();
        }

        public void ValidateConfig() {
            if (!string.IsNullOrWhiteSpace(NotificationEmail)) {
                if(!NotificationEmail!.Contains("@")) {
                    throw new ArgumentException("Invalid email address", nameof(NotificationEmail));
                }
                if(string.IsNullOrEmpty(SmtpServer)) {
                    throw new ArgumentException("SMTP server not configured", nameof(SmtpServer));
                }
                if(string.IsNullOrEmpty(SenderEmail)) {
                    throw new ArgumentException("Sender email not configured", nameof(SenderEmail));
                }
                if(string.IsNullOrEmpty(SenderPassword)) {
                    throw new ArgumentException("Sender password not configured", nameof(SenderPassword));
                }
                if(!SenderEmail!.Contains("@")) {
                    throw new ArgumentException("Invalid sender email address", nameof(SenderEmail));
                }
            }
        }

        public static Config ReadConfig(string filename = "config.json") {
            if(File.Exists(filename)) {
                var config = JsonSerializer.Deserialize<Config>(File.ReadAllText(filename)) ?? throw new Exception("Deserialized config was null!");
                Console.WriteLine($"Config read from {filename}. Attempting validation.");
                config.ValidateConfig();
                config.IsDefault = false;
                return config;
            }
            Console.WriteLine("Config file not found, default settings will be used.");
            return new Config();
        }

        public static void WriteConfig(string filename = "config.json") {
            File.WriteAllText(filename, JsonSerializer.Serialize(new Config(), new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
