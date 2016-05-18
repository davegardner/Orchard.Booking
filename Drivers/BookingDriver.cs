using Cascade.Booking.Models;
using Cascade.Booking.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using System;

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

        // IMPORT
        protected override void Importing(BookingPart part, ImportContentContext context)
        {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null)
            {
                return;
            }

            context.ImportAttribute(part.PartDefinition.Name, "BookingState", bs =>
                part.BookingState = (BookingState)Enum.Parse(typeof(BookingState), bs)
            );
            context.ImportAttribute(part.PartDefinition.Name, "RawGuests", rawGuests =>
                part.Record.RawGuests = rawGuests
            );
            context.ImportAttribute(part.PartDefinition.Name, "Name", name=>
                part.Name = name
            );
        }

        // EXPORT
        protected override void Exporting(BookingPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("BookingState", part.BookingState.ToString());
            context.Element(part.PartDefinition.Name).SetAttributeValue("RawGuests", part.Record.RawGuests);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Name", part.Name);
        }
    }

}
