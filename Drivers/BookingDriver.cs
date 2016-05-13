using Cascade.Booking.Models;
using Cascade.Booking.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;

namespace Cascade.Booking.Drivers
{
    public class BookingDriver : ContentPartDriver<BookingPart>
    {
        private readonly IBookingService _bookingService;
        public Localizer T { get; set; }

        public BookingDriver(IBookingService bookingService)
        {
            T = NullLocalizer.Instance;
            _bookingService = bookingService;
        }

        protected override string Prefix
        {
            get
            {
                return "Booking";
            }
        }

        protected override DriverResult Display(BookingPart part, string displayType, dynamic shapeHelper)
        {
            return Combined(
                ContentShape("Parts_Booking", () => shapeHelper.Parts_Booking(
                    BookingId: part.Id, Booking: part)),
                ContentShape("Parts_Booking_Summary", () => shapeHelper.Parts_Booking_Summary(
                    BookingId: part.Id, Booking: part))
                );

        }

        //GET
        protected override DriverResult Editor(BookingPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_Booking_Edit",
                                () => shapeHelper.EditorTemplate(
                                    TemplateName: "Parts/Booking",
                                    Model: part,
                                    Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(BookingPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var result = updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }

}
