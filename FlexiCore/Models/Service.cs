using System.ComponentModel.DataAnnotations;

namespace FlexiCore.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public decimal Price { get; set; }
        public int BusinessId { get; set; }
    }
}
