using System.ComponentModel.DataAnnotations;

namespace OpenTableApp.Models
{
    public class RestaurantFilterViewModel
    {
        public int? MetropolisId { get; set; }
        public PriceRange? PriceRange { get; set; }
        public string? CuisineStyle { get; set; }

        public List<Metropolis> Metropolises { get; set; } = new();
        public List<string> CuisineStyles { get; set; } = new();
        public List<Restaurant> Restaurants { get; set; } = new();
    }
}
