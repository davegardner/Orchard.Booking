using Cascade.Booking.Models;
using Cascade.Booking.Services;
using Cascade.Booking.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
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

        public AdminController(IOrchardServices services, IContentManager contentManager, IBookingService bookingService, ISiteService siteService, IShapeFactory shapeFactory)
        {
            Services = services;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            bs = bookingService;
            ss = siteService;
            Shape = shapeFactory;
            cm = contentManager;
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
                            bs.Delete(bookingSummaryViewModel.Part.Id);
                        }
                        break;
                    case BookingBulkAction.ChangeStateToTentative:
                        foreach (var bookingSummaryViewModel in checkedEntries)
                        {
                            bs.SetBookingState(bookingSummaryViewModel.Part.Id, BookingState.Tentative);
                        }
                        break;
                    case BookingBulkAction.ChangeStateToFirm:
                        foreach (var bookingSummaryViewModel in checkedEntries)
                        {
                            bs.SetBookingState(bookingSummaryViewModel.Part.Id, BookingState.Firm);
                        }
                        break;
                    case BookingBulkAction.ChangeStateToCancelled:
                        foreach (var bookingSummaryViewModel in checkedEntries)
                        {
                            bs.SetBookingState(bookingSummaryViewModel.Part.Id, BookingState.Cancelled);
                        }
                        break;
                    case BookingBulkAction.ChangeStateToStandby:
                        foreach (var bookingSummaryViewModel in checkedEntries)
                        {
                            bs.SetBookingState(bookingSummaryViewModel.Part.Id, BookingState.Standby);
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
                case BookingBulkFilter.Open:
                    bookings = bookings.Where(p => p.BookingState != BookingState.Cancelled);
                    break;
                case BookingBulkFilter.Closed:
                    bookings = bookings.Where(p => p.BookingState == BookingState.Cancelled);
                    break;
            }

            bookings = bookings.OrderByDescending(p => p.Id)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);



            foreach (var booking in bookings.ToList())
            {
                var bookingSummaryVm = new BookingSummaryViewModel
                {
                    Part = booking,
                    IsChecked = false
                };
                model.Bookings.Add(bookingSummaryVm);
            }
            model.Pager = Shape.Pager(pager).TotalItemCount(bs.GetAllBookings().Count());
            model.Options = options;
            return View(model);
        }

        /////////////////// CREATE ///////////////////////
        public ActionResult Create()
        {
            var bookingVm = new BookingDetailsViewModel
            {
                Year = DateTime.Now.Year
            };
            return View(bookingVm);
        }

        [HttpPost]
        public ActionResult Create(BookingDetailsViewModel bookingVm)
        {
            if (ProcessBookingPersistence(bookingVm))
            {
                Services.Notifier.Information(T("New Booking saved."));
                return View(bookingVm);
                //return RedirectToAction("Index");
            }
            //booking.StartDate.ShowDate = true;
            //booking.StartDate.ShowTime = false;
            //booking.EndDate.ShowDate = true;
            //booking.EndDate.ShowTime = false;

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
                //MaxAnswers = booking.MaxAnswers,
                //PollState = booking.PollState,
                //Question = booking.Question,
                //StartDate = new DateTimeEditor
                //{
                //    Date = _dateLocalizationServices.ConvertToLocalizedDateString(booking.StartDate),
                //    ShowDate = true,
                //    ShowTime = false
                //},
                //EndDate = new DateTimeEditor
                //{
                //    Date = _dateLocalizationServices.ConvertToLocalizedDateString(booking.EndDate),
                //    ShowDate = true,
                //    ShowTime = false
                //}

                Name = booking.Name,
                Year = booking.Year,
                Guests = booking.Guests,
                BookingState = booking.BookingState
            };
            //foreach (var answer in booking.Answers)
            //{
            //    var answerModel = new PollAnswerViewModel
            //    {
            //        Id = answer.Id,
            //        Answer = answer.Answer,
            //        Votes = answer.Votes
            //    };
            //    bookingDetailsViewModel.Answers.Add(answerModel);
            //}
            return View(bookingVm);
        }

        [HttpPost]
        public ActionResult Edit(BookingDetailsViewModel bookingVm)
        {
            if (ProcessBookingPersistence(bookingVm))
            {
                Services.Notifier.Information(T("Changes to Booking saved."));
                return View(bookingVm);
                //return RedirectToAction("Index");
            }
            //booking.StartDate.ShowDate = true;
            //booking.StartDate.ShowTime = false;
            //booking.EndDate.ShowDate = true;
            //booking.EndDate.ShowTime = false;

            return View(bookingVm);
        }


        /////////////////// AJAX ///////////////////////

        public ActionResult GetNewGuest(int sequence)
        {
            return PartialView("EditorTemplates/Guest", new Guest
            {
                Sequence = sequence,
                From = DateTime.Now,
                To = DateTime.Now + new TimeSpan(1, 0, 0, 0),
                Category = GuestCategory.Adult
            });
        }

        /////////////////// HELPERS ///////////////////////

        private bool ProcessBookingPersistence(BookingDetailsViewModel bookingVm)
        {
            if (ModelState.IsValid)
            {
                var bookingPart = bs.Get(bookingVm.Id) ?? cm.Create<BookingPart>("Booking");
                bookingPart.BookingState = bookingVm.BookingState;
                bookingPart.Name = bookingVm.Name;
                bookingPart.Year = bookingVm.Year;

                bookingPart.Guests = bookingVm.Guests == null ? null : bookingVm.Guests.Where(g => !g.Deleted);

                bs.SaveOrUpdate(bookingPart);
            }
            return ModelState.IsValid;
        }

    }
}
