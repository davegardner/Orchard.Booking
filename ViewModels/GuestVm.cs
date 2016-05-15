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
        public int Sequence { get; set; }    // not persisted
        public bool Deleted { get; set; }   // not persisted
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public GuestCategory Category { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public Decimal CostPerNight { get; set; }
        public Decimal? TotalCost
        {
            get { return CostPerNight * NumberOfNights; }
        }

        public int? NumberOfNights
        {
            get
            {
                int? nights = 0;
                try
                {
                    nights = Convert.ToInt32(Math.Round((DateTime.Parse(To) - DateTime.Parse(From)).TotalDays, 0));
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

    }
}