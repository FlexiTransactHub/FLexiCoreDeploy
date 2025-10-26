namespace FlexiCore.Models
{
    public class BusinessTypeConfiguration
    {
        public int Id { get; set; }
        public string BusinessType { get; set; } // e.g., Restaurant, Pharmacy
        public string SettingsJson { get; set; } // JSON for custom settings
    }
}
