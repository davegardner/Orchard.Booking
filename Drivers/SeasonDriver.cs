using Cascade.Booking.Models;
using Cascade.Booking.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;

namespace Cascade.Booking.Drivers
{
    public class SeasonDriver : ContentPartDriver<SeasonPart>
    {
        private readonly IBookingService _bookingService;
        public Localizer T { get; set; }

        public SeasonDriver(IBookingService bookingService)
        {
            T = NullLocalizer.Instance;
            _bookingService = bookingService;
        }

        protected override string Prefix
        {
            get
            {
                return "Season";
            }
        }

        protected override DriverResult Display(SeasonPart part, string displayType, dynamic shapeHelper)
        {
            return Combined(
                ContentShape("Parts_Season", () => shapeHelper.Parts_Season(
                    SeasonId: part.Id, Season: part)),
                ContentShape("Parts_Season_Summary", () => shapeHelper.Parts_Season_Summary(
                    SeasonId: part.Id, Season: part))
                );

        }

        //GET
        protected override DriverResult Editor(SeasonPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Season_Edit",
                                () => shapeHelper.EditorTemplate(
                                    TemplateName: "Parts/Season",
                                    Model: part,
                                    Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(SeasonPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var result = updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }

}
