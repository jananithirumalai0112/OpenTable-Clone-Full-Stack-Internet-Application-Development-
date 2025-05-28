using System;
using System.Collections.Generic;

namespace OpenTableApp.Models
{
    public class ReservationDetailViewModel
    {
        public Restaurant? Restaurant { get; set; }
        public List<string> AvailableTimeSlots { get; set; } = new List<string>();
        public string? SelectedTime { get; set; }
        public DateTime ReservationDate { get; set; } = DateTime.Today;
        public int NumberOfPeople { get; set; }
    }
}
