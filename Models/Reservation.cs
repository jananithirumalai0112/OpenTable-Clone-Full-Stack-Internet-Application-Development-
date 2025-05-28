using System;
using System.ComponentModel.DataAnnotations;

namespace OpenTableApp.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public int RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }

        [Required]
        public DateTime ReservationDate { get; set; }

        [Required]
        public string? ReservationTime { get; set; }

        [Required]
        public int NumberOfPeople { get; set; }

        [Required]
        public string? UserId { get; set; }
        public User? User { get; set; }

        // Add the IsConfirmed property
        public bool IsConfirmed { get; set; } // This will default to 'false' when a reservation is created
    }
}
