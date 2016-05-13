using Cascade.Booking.Models;
using Cascade.Booking.Services;
using Cascade.Booking.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.ViewModels;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Localization.Services;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.Themes;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Cascade.Booking.Controllers
{
    [Themed]
    [Admin]
    public class AdminSeasonController : Controller
    {
        public IOrchardServices Services { get; set; }
        public ILogger Logger { get; set; }
        private readonly IBookingService bs;
        private readonly ISiteService ss;
        dynamic Shape { get; set; }
        private readonly IContentManager cm;
        private readonly IDateLocalizationServices dls;

        public AdminSeasonController(IOrchardServices services, IContentManager contentManager, IBookingService bookingService, ISiteService siteService, IShapeFactory shapeFactory, IDateLocalizationServices _dateLocalizationServices)
        {
            Services = services;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            bs = bookingService;
            ss = siteService;
            Shape = shapeFactory;
            cm = contentManager;
            dls = _dateLocalizationServices;
        }

        public Localizer T { get; set; }

        [HttpPost]
        [Orchard.Mvc.FormValueRequired("submit.BulkEdit")]
        public ActionResult Index(FormCollection input)
        {
            var viewModel = new SeasonIndexViewModel { Options = new SeasonIndexOptions() };
            TryUpdateModel(viewModel);
            try
            {
                IEnumerable<SeasonSummaryViewModel> checkedEntries = viewModel.Seasons.Where(c => c.IsChecked);
                switch (viewModel.Options.BulkAction)
                {
                    case SeasonBulkAction.None:
                        Services.Notifier.Add(NotifyType.Information, T("Did nothing. Item count: " + checkedEntries.Count()));
                        break;
                    case SeasonBulkAction.Delete:
                        foreach (var seasonSummaryViewModel in checkedEntries)
                        {
                            bs.DeleteSeason(seasonSummaryViewModel.Part.Id);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Error, exception, "Editing Season failed: {0}", exception.Message);
                return RedirectToAction("Index", "AdminSeason", new { options = viewModel.Options });
            }
            return RedirectToAction("Index");
        }

        public ActionResult Index(SeasonIndexOptions options, PagerParameters pagerParameters)
        {
            var model = new SeasonIndexViewModel();
            var pager = new Pager(ss.GetSiteSettings(), pagerParameters);

            // Default options
            if (options == null)
                options = new SeasonIndexOptions();

            var seasons = bs.GetAllSeasons();

            switch (options.Filter)
            {
                case SeasonBulkFilter.All:
                    break;
                    //case SeasonBulkFilter.Open:
                    //    seasons = seasons.Where(p => p.BookingState != SeasonState.Cancelled);
                    //    break;
                    //case SeasonBulkFilter.Closed:
                    //    seasons = seasons.Where(p => p.BookingState == SeasonState.Cancelled);
                    //    break;
            }

            seasons = seasons.OrderByDescending(p => p.Id)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);

            foreach (var season in seasons.ToList())
            {
                var seasonModel = new SeasonSummaryViewModel
                {
                    Part = season,
                    IsChecked = false
                };
                model.Seasons.Add(seasonModel);
            }
            model.Pager = Shape.Pager(pager).TotalItemCount(bs.GetAllSeasons().Count());
            model.Options = options;
            return View(model);
        }

        /////////////////// CREATE ///////////////////////
        public ActionResult Create()
        {
            var seasonDetailsViewModel = new SeasonDetailsViewModel
            {
                From = new DateTimeEditor
                {
                    //Date = dls.ConvertToLocalizedDateString(DateTime.Now),
                    ShowDate = true,
                    ShowTime = false
                },
                To = new DateTimeEditor
                {
                    //Date = dls.ConvertToLocalizedDateString(DateTime.Now),
                    ShowDate = true,
                    ShowTime = false
                }
            };
            return View(seasonDetailsViewModel);
        }

        [HttpPost]
        public ActionResult Create(SeasonDetailsViewModel season)
        {
            if (ProcessSeasonPersistence(season))
            {
                return RedirectToAction("Index");
            }

            return View(season);
        }

        /////////////////// DELETE ///////////////////////

        public ActionResult Delete(int id)
        {
            bs.Delete(id);
            return RedirectToAction("Index");
        }

        /////////////////// EDIT ///////////////////////

        public ActionResult Edit(int id)
        {
            SeasonPart season = bs.GetSeason(id);
            var seasonDetailsViewModel = new SeasonDetailsViewModel
            {
                Id = season.Id,
                Title = season.Title,
                From = new DateTimeEditor
                {
                    Date = dls.ConvertToLocalizedDateString(season.From),
                    ShowDate = true,
                    ShowTime = false
                },
                To = new DateTimeEditor
                {
                    Date = dls.ConvertToLocalizedDateString(season.To),
                    ShowDate = true,
                    ShowTime = false
                },
                Rate = season.Rate
            };
            return View(seasonDetailsViewModel);
        }

        [HttpPost]
        public ActionResult Edit(SeasonDetailsViewModel season)
        {
            if (ProcessSeasonPersistence(season))
            {
                return RedirectToAction("Index");
            }
            return View(season);
        }


        /////////////////// HELPERS ///////////////////////

        private bool ProcessSeasonPersistence(SeasonDetailsViewModel seasonDetailsVm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var from = dls.ConvertFromLocalizedString(seasonDetailsVm.From.Date + " " + "00:00:00").Value;
                    var to = dls.ConvertFromLocalizedString(seasonDetailsVm.To.Date + " " + "00:00:00").Value;
                    var seasonPart = bs.GetSeason(seasonDetailsVm.Id) ?? cm.Create<SeasonPart>("Season");
                    seasonPart.Title = seasonDetailsVm.Title;
                    seasonPart.From = from;
                    seasonPart.To = to;
                    seasonPart.Rate = seasonDetailsVm.Rate;
                    bs.SaveOrUpdateSeason(seasonPart);
                }
                catch (FormatException)
                {
                    ModelState.AddModelError("InvalidDate", "Invalid date");
                }
            }
            return ModelState.IsValid;
        }

    }
}
