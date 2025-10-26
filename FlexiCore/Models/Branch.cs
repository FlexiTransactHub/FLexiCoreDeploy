using System.ComponentModel.DataAnnotations;

namespace FlexiCore.Models
{
    public class Branch
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Address { get; set; }
        public int BusinessId { get; set; }
    }
}
