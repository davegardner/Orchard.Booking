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

            var today = clock.UtcNow;
            var season = bs.GetAllSeasons().FirstOrDefault(s => s.From <= today && today <= s.To);

            var vm = new GuestVm
            {
                // reasonable defaults
                SeasonName = season.Title,
                From = new DateTimeEditor { ShowDate = true, ShowTime = false, Date = dls.ConvertToLocalizedDateString(clock.UtcNow) },
                To = new DateTimeEditor { ShowDate = true, ShowTime = false, Date = dls.ConvertToLocalizedDateString(clock.UtcNow.AddDays(2.0)) },
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

            var shape = sf.Edit_Guest(
                Guest: vm
            );

            return new ShapeResult(this, shape);
        }

        [HttpPost]
        public ActionResult Update(GuestVm Guest, string ReturnUrl)
        {
            var booking = bs.Get(Guest.BookingId, Services.WorkContext.CurrentUser);
            if (booking == null || !Services.Authorizer.Authorize(Permissions.AddBooking, T("Please log in ")))
                return Redirect("~/Users/Account/AccessDenied?ReturnUrl=~/Booking/Bookings");

            var guest = booking.Guests.FirstOrDefault(g => g.Id == Guest.Id);
            if (guest == null || guest.Id == 0)
            {
                // add a new guest
                guest = new Guest();
                var guestList = booking.Guests.ToList();
                guestList.Add(guest);
                booking.Guests = guestList;
            }
            guest.Category = Guest.Category;
            guest.CostPerNight = Guest.CostPerNight;
            guest.FirstName = Guest.FirstName;
            guest.LastName = Guest.LastName;
            guest.From = dls.ConvertFromLocalizedDateString(Guest.From.Date);
            guest.To = dls.ConvertFromLocalizedDateString(Guest.To.Date);

            bs.NormalizeGuestBookings(booking);
            return RedirectToAction("Edit", "Booking", new { Id = booking.Id});
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
    }
}