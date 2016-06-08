using Cascade.Booking.Models;
using Orchard.Core.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cascade.Booking.ViewModels
{
    public class GuestVm
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int Sequence { get; set; }    // not persisted
        public bool Deleted { get; set; }   // not persisted
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public GuestCategory Category { get; set; }
        public DateTimeEditor  From { get; set; }
        public DateTimeEditor To { get; set; }
        public string SeasonName { get; set; }
        public Decimal CostPerNight { get; set; }
        public Decimal? TotalCost
        {
            get { return CostPerNight * NumberOfNights; }
        }

        public IList<DayVm> Days { get; set; }

        public int? NumberOfNights
        {
            get
            {
                int? nights = 0;
                try
                {
                    nights = Convert.ToInt32(Math.Round((DateTime.Parse(To.Date) - DateTime.Parse(From.Date)).TotalDays, 0));
                }
                catch
                {
                    nights = null;
                }
                return nights;
            }
        }
        public IEnumerable<SelectListItem> Categories 
        {
            get
            {
                return Enum.GetNames(typeof(GuestCategory)).Select(
                    e => new SelectListItem { Text = e, Value = e });
            }
        }

        public GuestVm()
        {
            From = new DateTimeEditor { ShowDate = true, ShowTime = false };
            To = new DateTimeEditor { ShowDate = true, ShowTime = false };
        }
    }
}