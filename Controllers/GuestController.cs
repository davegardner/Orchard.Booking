using Cascade.Booking.Models;
using Cascade.Booking.Services;
using Cascade.Booking.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.ViewModels;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Localization.Services;
using Orchard.Mvc;
using Orchard.Services;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Cascade.Booking.Controllers
{
    public class GuestController : Controller
    {
        private Localizer T { get; set; }
        public IOrchardServices Services { get; set; }
        private readonly IContentManager cm;
        private readonly IBookingService bs;
        private readonly dynamic sf;
        private readonly IDateLocalizationServices dls;
        private readonly IClock clock;

        public GuestController(IContentManager contentManager, IBookingService bookingService, IShapeFactory shapefactory, IOrchardServices services, IDateLocalizationServices dateLocalization, IClock clockService)
        {
            Services = services;
            cm = contentManager;
            bs = bookingService;
            sf = shapefactory;
            dls = dateLocalization;
            clock = clockService;

            T = NullLocalizer.Instance;
        }

        [Themed]
        public ActionResult Add(int BookingId)
        {
            var booking = bs.Get(BookingId, Services.WorkContext.CurrentUser);
            if (booking == null || !Services.Authorizer.Authorize(Permissions.AddBooking, T("Please log in ")))
                return Redirect("~/Users/Account/AccessDenied?ReturnUrl=~/Booking/Bookings");

            var today = bs.TodayUtc();
            var season = bs.GetAllSeasons().FirstOrDefault(s => s.From <= today && today <= s.To);

            var vm = new GuestVm
            {
                // reasonable defaults
                SeasonName = season.Title,
                From = new DateTimeEditor
                {
                    ShowDate = true,
                    ShowTime = false,
                    Date = dls.ConvertToLocalizedDateString(today.AddDays(1.0))
                },
                To = new DateTimeEditor
                {
                    ShowDate = true,
                    ShowTime = false,
                    Date = dls.ConvertToLocalizedDateString(today.AddDays(3.0))
                },
                Category = GuestCategory.Adult,
                CostPerNight = season.Rate,
                BookingId = booking.Id
            };

            var shape = sf.Edit_Guest(
                Guest: vm
            );

            return new ShapeResult(this, shape);
        }

        [Themed]
        public ActionResult Edit(int BookingId, int GuestId)
        {
            var booking = bs.Get(BookingId, Services.WorkContext.CurrentUser);
            if (booking == null || !Services.Authorizer.Authorize(Permissions.AddBooking, T("Please log in ")))
                return Redirect("~/Users/Account/AccessDenied?ReturnUrl=~/Booking/Bookings");

            var guest = booking.Guests.FirstOrDefault(g => g.Id == GuestId);
            var season = bs.GetSeason(guest);
            var vm = bs.ConvertToGuestVm(BookingId, guest, season);

            var shape = sf.Edit_Guest(Guest: vm);

            return new ShapeResult(this, shape);
        }

        [HttpPost]
        public ActionResult Update(GuestVm Guest, string ReturnUrl = null)
        {
            var booking = bs.Get(Guest.BookingId, Services.WorkContext.CurrentUser);
            if (booking == null || !Services.Authorizer.Authorize(Permissions.AddBooking, T("Please log in ")))
                return Redirect("~/Users/Account/AccessDenied?ReturnUrl=~/Booking/Bookings");

            var guest = booking.Guests.FirstOrDefault(g => g.Id == Guest.Id);
            if (guest == null || guest.Id == 0)
            {
                // get new id
                var id = booking.Guests.Count() == 0 ? 1 : booking.Guests.Max(b => b.Id) + 1;

                // add a new guest
                guest = new Guest { Id = id };
                var guestList = booking.Guests.ToList();
                guestList.Add(guest);
                booking.Guests = guestList;
            }
            guest.Category = Guest.Category;
            guest.CostPerNight = Guest.CostPerNight;
            guest.FirstName = Guest.FirstName;
            guest.LastName = Guest.LastName;
            guest.From = dls.ConvertFromLocalizedDateString(Guest.From.Date);

            // to checkout time
            Guest.To.ShowDate = true;
            Guest.To.ShowTime = true;
            guest.To = dls.ConvertFromLocalizedString(Guest.To.Date + " 09:59:59");

            if (Guest.Days != null)
            {
                guest.Days = Guest.Days.Select(d => new Day
                {
                    Id = d.Id,
                    Coupon = d.Coupon,
                    Cost = d.Cost,
                    Date = dls.ConvertFromLocalizedDateString(d.Date.Date)
                });
            }
            else
                guest.Days = new List<Day>();

            bs.SaveOrUpdateBooking(booking);

            if (this.Request.Form["submit.applyCoupons"] != null)
            {
                return RedirectToAction("Coupons", "Guest", new { BookingId = booking.Id, GuestId = guest.Id });
            }

            if (string.IsNullOrWhiteSpace(ReturnUrl))
                return RedirectToAction("Edit", "Booking", new { Id = booking.Id });

            return Redirect(ReturnUrl);
        }

        public ActionResult Delete(int BookingId, int GuestId)
        {
            var booking = bs.Get(BookingId, Services.WorkContext.CurrentUser);
            if (booking == null || !Services.Authorizer.Authorize(Permissions.AddBooking, T("Please log in ")))
                return Redirect("~/Users/Account/AccessDenied?ReturnUrl=~/Booking/Bookings");

            var guest = booking.Guests.FirstOrDefault(g => g.Id == GuestId);
            var guestList = booking.Guests.ToList();
            guestList.Remove(guest);
            booking.Guests = guestList;

            return RedirectToAction("Edit", "Booking", new { booking.Id });
        }

        [Themed]
        public ActionResult Coupons(int BookingId, int GuestId)
        {
            var booking = bs.Get(BookingId, Services.WorkContext.CurrentUser);
            if (booking == null || !Services.Authorizer.Authorize(Permissions.AddBooking, T("Please log in ")))
                return Redirect("~/Users/Account/AccessDenied?ReturnUrl=~/Booking/Bookings");

            var from = bs.TodayUtc();
            var defaultRate = bs.GetRate(from);
            var guest = booking.Guests.FirstOrDefault(g => g.Id == GuestId);
            if (guest == null)
                guest = new Guest
                {
                    FirstName = "Guest",
                    From = from,
                    To = from.AddDays(2),
                    CostPerNight = defaultRate,
                    Days = new List<Day>()
                };
            var season = bs.GetSeason(guest);
            var vm = bs.ConvertToGuestVm(BookingId, guest, season);

            // Assumption: Date Range (From to) has been correctly set up
            if (!guest.From.HasValue || !guest.To.HasValue
                || guest.From.Value > guest.To.Value)
                throw new InvalidOperationException("Invalid Guest From/To dates");

            // Create a set of days corresponding to the date range and then
            // initialze set members to either existing values, or if no existing value
            // then the default.
            var days = new List<Day>();
            int numDays = guest.NumberOfNights ?? 0;
            var existingDays = guest.Days.ToDictionary(d => d.Date);
            for (int i = 0; i < numDays; i++)
            {
                var date = guest.From.Value.AddDays(i);
                var day = new Day
                {
                    Id = i,
                    Date = date,
                    Cost = bs.GetRate(date),
                    Coupon = false
                };
                Day exDay = null;
                existingDays.TryGetValue(day.Date, out exDay);
                if (exDay != null)
                {
                    day.Cost = exDay.Cost;
                    day.Coupon = exDay.Coupon;
                }
                days.Add(day);
            }

            vm.Days = days.Select(d =>
                new DayVm
                {
                    Cost = d.Cost,
                    Date = new DateTimeEditor
                    {
                        Date = dls.ConvertToLocalizedDateString(d.Date),
                        ShowDate = true,
                        ShowTime = false
                    },
                    Coupon = d.Coupon,
                    Id = d.Id
                }
            ).ToList();

            var shape = sf.Edit_Coupons(
                Guest: vm
            );

            return new ShapeResult(this, shape);
        }
    }
}