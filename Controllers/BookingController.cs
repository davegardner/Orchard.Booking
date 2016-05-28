using Cascade.Booking.Models;
using Cascade.Booking.Services;
using Cascade.Booking.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Themes;
using System.Linq;
using System.Web.Mvc;

namespace Cascade.Booking.Controllers
{
    public class BookingController : Controller
    {
        private Localizer T { get; set; }
        public IOrchardServices Services { get; set; }
        private readonly IContentManager cm;
        private readonly IBookingService bs;
        private readonly dynamic sf;

        public BookingController(IContentManager contentManager, IBookingService bookingService, IShapeFactory shapefactory, IOrchardServices services)
        {
            Services = services;
            cm = contentManager;
            bs = bookingService;
            sf = shapefactory;
            T = NullLocalizer.Instance;
        }


        [HttpPost]
        public JsonResult Verify(BookingPart booking)
        {
            bs.NormalizeGuestBookings(booking);
            return GetBookingDetails(booking);
        }


        [HttpPost]
        public JsonResult Save(BookingPart booking)
        {
            cm.Publish(booking.ContentItem);
            return Json(true);
        }

        public JsonResult Get(int id)
        {
            var booking = cm.Get<BookingPart>(id);
            return GetBookingDetails(booking);
        }

        public JsonResult GetBookingDetails(BookingPart booking)
        {
            // need an intermediate ViewModel because otherwise we get a circular ref.
            var vm = new
            {
                id = booking.Id,
                name = booking.Name,
                guests = (from guest in booking.Guests
                          select new
                          {
                              deleted = guest.Deleted,
                              lastName = guest.LastName,
                              firstName = guest.FirstName,
                              category = guest.Category,
                              fromo = guest.From,
                              to = guest.To,
                              costPerNight = guest.CostPerNight,
                              totalCost = guest.TotalCost
                          }).ToArray()
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [Themed]
        public ActionResult Bookings()
        {
            if (!Services.Authorizer.Authorize(Permissions.AddBooking, T("Please log in ")))
                return Redirect("~/Users/Account/AccessDenied?ReturnUrl=~/Booking/Bookings");

            var user = Services.WorkContext.CurrentUser;
            if (user == null)
                return null;

            var bookings = bs.GetBookings(user);

            var shape = sf.User_Bookings(
                User: user,
                Bookings: bookings
            );
            return new ShapeResult(this, shape);
        }

        [Themed]
        public ActionResult Add()
        {
            if (!Services.Authorizer.Authorize(Permissions.AddBooking, T("Please log in ")))
                return Redirect("~/Users/Account/AccessDenied?ReturnUrl=~/Booking/Bookings");

            // Actually create the ContextItem so it has an Id
            var booking = cm.New<BookingPart>("Booking");
            cm.Create(booking.ContentItem);

            var shape = sf.Edit_Booking(
                Booking: booking
            );

            return new ShapeResult(this, shape);
        }

        [Themed]
        public ActionResult Edit(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.AddBooking, T("Please log in ")))
                return Redirect("~/Users/Account/AccessDenied?ReturnUrl=~/Booking/Bookings");

            var booking = bs.Get(id, Services.WorkContext.CurrentUser);
            if (booking == null)
                return Redirect("~/Users/Account/AccessDenied?ReturnUrl=~/Booking/Bookings");

            var shape = sf.Edit_Booking(
                Booking: booking
            );

            return new ShapeResult(this, shape);
        }

        public ActionResult Delete(int id)
        {
            if (!Services.Authorizer.Authorize(Permissions.AddBooking, T("Please log in ")))
                return Redirect("~/Users/Account/AccessDenied?ReturnUrl=~/Booking/Bookings");

            var booking = bs.Get(id, Services.WorkContext.CurrentUser);
            if (booking == null)
                return Redirect("~/Users/Account/AccessDenied?ReturnUrl=~/Booking/Bookings");

            cm.Remove(booking.ContentItem);

            Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("Deleted booking '{0}'", booking.Name));

            return RedirectToAction("Bookings");
        }

        [HttpPost]
        public ActionResult Update(BookingSummaryViewModel Booking)
        {
            if (!Services.Authorizer.Authorize(Permissions.AddBooking, T("Please log in ")))
                return Redirect("~/Users/Account/AccessDenied?ReturnUrl=~/Booking/Bookings");

            //bool newBooking = Booking.Id == 0;

            BookingPart booking = null;
            //if (newBooking)
            //    booking = cm.New<BookingPart>("Booking");
            //else
            //{
                booking = bs.Get(Booking.Id, Services.WorkContext.CurrentUser);
                if (booking == null)
                    return Redirect("~/Users/Account/AccessDenied?ReturnUrl=~/Booking/Bookings");
            //}

            booking.Name = Booking.Name;
            booking.BookingState = Booking.BookingState;

            //if (newBooking)
            //    cm.Create(booking.ContentItem);

            Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("Booking '{0}' saved.", booking.Name));

            return RedirectToAction("Bookings");
        }
    }
}