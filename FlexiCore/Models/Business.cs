using System.ComponentModel.DataAnnotations;

namespace FlexiCore.Models
{
    public class Business
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string OwnerId { get; set; } // Links to IdentityUser
        public ApplicationUser Owner { get; set; }

        public string BusinessType { get; set; } // e.g., Restaurant, Pharmacy, Supermarket

    }
}
