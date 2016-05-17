using Cascade.Booking.Models;
using Cascade.Booking.Services;
using Cascade.Booking.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.ViewModels;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.Logging;
using Orchard.Services;
using Orchard.Settings;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace Cascade.Booking.Controllers
{
    public class AdminController : Controller
    {
        public IOrchardServices Services { get; set; }
        public ILogger Logger { get; set; }
        private readonly IBookingService bs;
        private readonly ISiteService ss;
        dynamic Shape { get; set; }
        private readonly IContentManager cm;
        private readonly IDateLocalizationServices dls;
        private readonly IClock clock;

        public AdminController(IOrchardServices services, IContentManager contentManager, IBookingService bookingService, ISiteService siteService, IShapeFactory shapeFactory, IDateLocalizationServices _dateLocalizationServices, IClock orchardClock)
        {
            Services = services;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            bs = bookingService;
            ss = siteService;
            Shape = shapeFactory;
            cm = contentManager;
            dls = _dateLocalizationServices;
            clock = orchardClock;
        }

        public Localizer T { get; set; }

        [HttpPost]
        [Orchard.Mvc.FormValueRequired("submit.BulkEdit")]
        public ActionResult Index(FormCollection input)
        {
            var viewModel = new BookingIndexViewModel { Options = new BookingIndexOptions() };
            TryUpdateModel(viewModel);
            try
            {
                IEnumerable<BookingSummaryViewModel> checkedEntries = viewModel.Bookings.Where(c => c.IsChecked);
                switch (viewModel.Options.BulkAction)
                {
                    case BookingBulkAction.None:
                        Services.Notifier.Add(NotifyType.Information, T("Did nothing. Item count: " + checkedEntries.Count()));
                        break;
                    case BookingBulkAction.Delete:
                        foreach (var bookingSummaryViewModel in checkedEntries)
                        {
                            bs.Delete(bookingSummaryViewModel.Id);
                        }
                        break;
                    case BookingBulkAction.ChangeStateToTentative:
                        foreach (var bookingSummaryViewModel in checkedEntries)
                        {
                            bs.SetBookingState(bookingSummaryViewModel.Id, BookingState.Tentative);
                        }
                        break;
                    case BookingBulkAction.ChangeStateToFirm:
                        foreach (var bookingSummaryViewModel in checkedEntries)
                        {
                            bs.SetBookingState(bookingSummaryViewModel.Id, BookingState.Firm);
                        }
                        break;
                    case BookingBulkAction.ChangeStateToCancelled:
                        foreach (var bookingSummaryViewModel in checkedEntries)
                        {
                            bs.SetBookingState(bookingSummaryViewModel.Id, BookingState.Cancelled);
                        }
                        break;
                    case BookingBulkAction.ChangeStateToStandby:
                        foreach (var bookingSummaryViewModel in checkedEntries)
                        {
                            bs.SetBookingState(bookingSummaryViewModel.Id, BookingState.Standby);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Error, exception, "Editing booking failed: {0}", exception.Message);
                return RedirectToAction("Index", "Admin", new { options = viewModel.Options });
            }
            return RedirectToAction("Index");
        }

        public ActionResult Index(BookingIndexOptions options, PagerParameters pagerParameters)
        {
            var model = new BookingIndexViewModel();
            var pager = new Pager(ss.GetSiteSettings(), pagerParameters);

            // Default options
            if (options == null)
                options = new BookingIndexOptions();

            var bookings = bs.GetAllBookings(2016);

            switch (options.Filter)
            {
                case BookingBulkFilter.All:
                    break;
                case BookingBulkFilter.Tentative:
                    bookings = bookings.Where(p => p.BookingState == BookingState.Tentative);
                    break;
                case BookingBulkFilter.Firm:
                    bookings = bookings.Where(p => p.BookingState == BookingState.Firm);
                    break;
                case BookingBulkFilter.Cancelled:
                    bookings = bookings.Where(p => p.BookingState == BookingState.Cancelled);
                    break;
                case BookingBulkFilter.Standby:
                    bookings = bookings.Where(p => p.BookingState == BookingState.Standby);
                    break;
            }

            bookings = bookings.OrderByDescending(p => p.Id)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);

            model.Bookings = bookings.Select(b => new BookingSummaryViewModel
            {
                Id = b.Id,
                Name = b.Name,
                BookingState = b.BookingState,
                Guests = b.Guests,
                IsChecked = false
            }).ToList();

            model.Pager = Shape.Pager(pager).TotalItemCount(bs.GetAllBookings().Count());
            model.Options = options;
            return View(model);
        }

        /////////////////// CREATE ///////////////////////
        public ActionResult Create()
        {
            var bookingVm = new BookingDetailsViewModel();
            return View(bookingVm);
        }

        [HttpPost]
        public ActionResult Create(BookingDetailsViewModel bookingVm)
        {
            if (PersistBooking(bookingVm))
            {
                Services.Notifier.Information(T("New Booking saved."));
                //return View(bookingVm);
                return RedirectToAction("Index");
            }

            return View(bookingVm);
        }

        /////////////////// DELETE ///////////////////////

        public ActionResult Delete(int id)
        {
            bs.Delete(id);
            Services.Notifier.Information(T("Booking deleted."));
            return RedirectToAction("Index");
        }

        /////////////////// EDIT ///////////////////////

        public ActionResult Edit(int id)
        {
            BookingPart booking = bs.Get(id);
            var bookingVm = new BookingDetailsViewModel
            {
                Id = booking.Id,
                Name = booking.Name,
                Guests = booking.Guests.Select(g => ConvertToGuestVm(g, bs.GetSeason(g))).ToList(),
                BookingState = booking.BookingState
            };
            return View(bookingVm);
        }

        [HttpPost]
        public ActionResult Edit(BookingDetailsViewModel bookingVm)
        {
            if (PersistBooking(bookingVm))
            {
                Services.Notifier.Information(T("Changes to Booking saved."));
                //return View(bookingVm);
                return RedirectToAction("Index");
            }
            return View(bookingVm);
        }


        /////////////////// AJAX ///////////////////////

        public ActionResult GetNewGuest(int sequence)
        {
            return PartialView("EditorTemplates/GuestVm", new GuestVm
            {
                Sequence = sequence,
                From = dls.ConvertToLocalizedDateString(clock.UtcNow),
                To = dls.ConvertToLocalizedDateString(clock.UtcNow + new TimeSpan(1, 0, 0, 0)),
                Category = GuestCategory.Adult
            });
        }

        /////////////////// HELPERS ///////////////////////

        private bool PersistBooking(BookingDetailsViewModel bookingVm)
        {
         

            if (ModelState.IsValid)
            {
                var bookingPart = bs.Get(bookingVm.Id) ?? cm.Create<BookingPart>("Booking");
                bookingPart.BookingState = bookingVm.BookingState;
                bookingPart.Name = bookingVm.Name;
                bookingPart.Guests = bookingVm.Guests == null ? null : bookingVm.Guests.Where(g => !g.Deleted).Select(g => ConvertToGuest(g)).ToList();

                // Validation
                if (GuestDatesOutOfRange(bookingVm))
                {
                    ModelState.AddModelError("GuestDateOutOfRange", "One or more guest dates are out of range");
                }

                var guestCount = bookingPart.Guests.Count();
                bs.SaveOrUpdateBooking(bookingPart);
                if (bookingPart.Guests.Count() != guestCount)
                {
                    var guestNames = DuplicatedGuests(bookingPart);
                    Services.Notifier.Information(T("One or more guest bookings have been duplicated because they crossed more than one season. Rates may be different for each season. The following guests are duplicated: " + guestNames));
                }
            }
            return ModelState.IsValid;
        }

        private bool GuestDatesOutOfRange(BookingDetailsViewModel bookingVm)
        {
            var from = bookingVm.Guests.Min(a => dls.ConvertFromLocalizedDateString(a.From));
            var to = bookingVm.Guests.Max(a => dls.ConvertFromLocalizedDateString(a.To));
            var seasons = bs.GetAllSeasons().ToList();
            if (from < seasons.Min(s => s.From) || to > seasons.Max(s => s.To))
                return true;
            return false;
        }

        private string DuplicatedGuests(BookingPart bookingPart)
        {
            var queryNames = from guest in bookingPart.Guests
                             group guest by guest.FirstName + " " + guest.LastName into guestGroup
                             orderby guestGroup.Key
                             select guestGroup;

            var duplicatedNames = new List<string>();
            foreach (var group in queryNames)
            {
                if (group.Count() > 1)
                    duplicatedNames.Add(group.Key);
            }
            return String.Join(", ", duplicatedNames);
        }

        private GuestVm ConvertToGuestVm(Guest guest, SeasonPart season)
        {
            var result = new GuestVm
            {
                Id = guest.Id,
                Sequence = 0,
                Deleted = false,
                LastName = guest.LastName,
                FirstName = guest.FirstName,
                Category = guest.Category,
                From = guest.From,
                To = guest.To,
                SeasonName = season.Title,
                CostPerNight = guest.CostPerNight
            };
            return result;
        }

        private Guest ConvertToGuest(GuestVm guest)
        {
            var result = new Guest
            {
                Id = guest.Id,
                Sequence = 0,
                Deleted = false,
                LastName = guest.LastName,
                FirstName = guest.FirstName,
                Category = guest.Category,
                From = guest.From,
                To = guest.To,
                CostPerNight = guest.CostPerNight
            };
            return result;
        }
    }
}
