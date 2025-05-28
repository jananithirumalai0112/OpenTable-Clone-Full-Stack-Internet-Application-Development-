using System.ComponentModel.DataAnnotations;

namespace OpenTableApp.Models
{
    public class Metropolis
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? State { get; set; }

        [Required]
        public string? Country { get; set; }

        public ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    }
}
