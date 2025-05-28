using System.ComponentModel.DataAnnotations;

namespace OpenTableApp.Models
{
    public class Restaurant
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        public string? Phone { get; set; }

        [Required]
        public string? OpenHours { get; set; }

        public string? ImagePath { get; set; }

        public int MetropolisId { get; set; }

        public Metropolis? Metropolis { get; set; }

        public PriceRange PriceRange { get; set; }

        [Required]
        public string? CuisineStyle { get; set; }
    }
}
