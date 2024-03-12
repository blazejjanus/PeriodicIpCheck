using System.Text.Json;

namespace PeriodicIpCheck {
    internal class Program {
        static void Main(string[] args) {
            try {
                Config config = Config.ReadConfig();
                var notificationService = new NotificationService(config);
                if (args.Length > 0) {
                    switch (args[0]) {
                        case "-h":
                        case "--help":
                            Console.WriteLine("Available options:\n\tno option will mean normal ip check.\n\t-c --config - display current config or write default if it's not present");
                            break;
                        case "-c":
                        case "--config":
                            DisplayConfig(config);
                            break;
                    }
                } else {
                    IpRecord ipRecord = GetIpRecord().GetAwaiter().GetResult();
                    Console.WriteLine(ipRecord.ToString());
                    IpRecord? lastRecord = GetLastRecord();
                    if(lastRecord == null) {
                        SaveLastRecord(ipRecord);
                        Console.WriteLine("No previous record found. Saving current record and exiting.");
                        notificationService.SendNotification(ipRecord, lastRecord);
                    } else {
                        if(ipRecord.IP != lastRecord.IP) {
                            SaveLastRecord(ipRecord);
                            Console.WriteLine("IP has changed. Saving current record and sending notification.");
                            File.AppendAllText(config.ResultsFilePath, ipRecord.ToString() + "\tIP has changed.\n");
                            notificationService.SendNotification(ipRecord, lastRecord);
                        } else {
                            Console.WriteLine("IP has NOT changed. Exiting.");
                            if(!config.LogOnlyChanges) {
                                File.AppendAllText(config.ResultsFilePath, ipRecord.ToString() + "\tIP has NOT changed.\n");
                            }
                        }
                    }
                }
            } catch (Exception exc) {
                Console.WriteLine("Error occurred: " + exc.Message);
            }
        }

        static async Task<IpRecord> GetIpRecord() {
            using HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.myip.com");
            HttpResponseMessage response = await client.GetAsync("");
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IpRecord>(json) ?? throw new Exception("Failed to read api response!");
        }

        static void DisplayConfig(Config config) {
            if (config.IsDefault) {
                Console.WriteLine("Using default settings.");
                Console.WriteLine("Writing default settings to config file...");
                Config.WriteConfig();
            } else {
                Console.WriteLine(config.ToString());
            }
        }

        static IpRecord? GetLastRecord() {
            if (File.Exists(".last_ip.json")) {
                return JsonSerializer.Deserialize<IpRecord>(File.ReadAllText(".last_ip.json"));
            }
            return null;
        }

        static void SaveLastRecord(IpRecord record) {
            File.WriteAllText(".last_ip.json", JsonSerializer.Serialize(record));
        }
    }
}
