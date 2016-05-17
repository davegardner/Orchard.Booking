using Cascade.Booking.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Cascade.Booking.ViewModels
{
    public class BookingDetailsViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public BookingState BookingState { get; set; }
        //public int Year{ get; set; }
        public IEnumerable<GuestVm> Guests { get; set; }
        public IEnumerable<SelectListItem> BookingStates {
            get {
                return Enum.GetNames(typeof (BookingState)).Select(
                    e => new SelectListItem {Text = e, Value = e});
            }
        }
    }
}
