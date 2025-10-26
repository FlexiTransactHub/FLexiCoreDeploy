using System.ComponentModel.DataAnnotations;

namespace FlexiCore.Models
{
    public class POSDevice
    {
        public int Id { get; set; }

        [Required]
        public string DeviceName { get; set; }

        public string DeviceType { get; set; } // e.g., Terminal, Mobile
        public int BranchId { get; set; }
    }
}
