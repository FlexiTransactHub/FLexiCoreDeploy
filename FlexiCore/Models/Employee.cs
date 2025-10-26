using System.ComponentModel.DataAnnotations;

namespace FlexiCore.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Role { get; set; } // e.g., Cashier, Manager
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public string UserId { get; set; } // Links to IdentityUser for login
        
    }
}
