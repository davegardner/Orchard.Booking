using Orchard.Core.Common.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cascade.Booking.ViewModels
{
    public class SeasonDetailsViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public DateTimeEditor From { get; set; }
        public DateTimeEditor To { get; set; }
        public Decimal Rate { get; set; }
    }
}
