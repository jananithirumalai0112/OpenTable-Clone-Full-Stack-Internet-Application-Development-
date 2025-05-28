using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenTableApp.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; } // Primary Key

        // Foreign Key to Restaurant
        [Required]
        public int RestaurantId { get; set; }

        [ForeignKey("RestaurantId")]
        public Restaurant Restaurant { get; set; } = null!;  // Navigation property

        [Required]
        public string RestaurantName { get; set; } = string.Empty;

        public string? MetropolisName { get; set; }

        public string? Time { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Range(1, 20, ErrorMessage = "Number of people must be between 1 and 20.")]
        public int NumberOfPeople { get; set; }


        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public User User { get; set; } = new User(); 
    }
}
