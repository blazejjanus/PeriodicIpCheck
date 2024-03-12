using System.Text.Json.Serialization;

namespace PeriodicIpCheck {
    internal class IpRecord {
        [JsonPropertyName("ip")]
        public string IP { get; set; } = string.Empty;
        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;
        [JsonPropertyName("cc")]
        public string CountryCode { get; set; } = string.Empty;
        public DateTime? TimeStamp { get; set; }

        public IpRecord() {
            TimeStamp = DateTime.Now;
        }

        public override string ToString() {
            return ToString();
        }

        public string ToString(bool inline = true) {
            if(inline) {
                return $"Time: {TimeStamp} IP: {IP} Country: {Country} Country code: {CountryCode}";
            } else {
                return $"IpRecord:\n\tTime: {TimeStamp}\n\tIP: {IP}\n\tCountry: {Country}\n\tCountry code: {CountryCode}";
            }
        }
    }
}
