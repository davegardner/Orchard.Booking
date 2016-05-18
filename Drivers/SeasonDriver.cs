using Cascade.Booking.Models;
using Cascade.Booking.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.Localization.Services;
using System;
using System.Xml;

namespace Cascade.Booking.Drivers
{
    public class SeasonDriver : ContentPartDriver<SeasonPart>
    {
        private readonly IBookingService _bookingService;
        private readonly IDateLocalizationServices dls;
        public Localizer T { get; set; }

        public SeasonDriver(IBookingService bookingService, IDateLocalizationServices dateLocalizationServices)
        {
            T = NullLocalizer.Instance;
            _bookingService = bookingService;
            dls = dateLocalizationServices;
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

        // IMPORT
        protected override void Importing(SeasonPart part, ImportContentContext context)
        {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null)
            {
                return;
            }

            context.ImportAttribute(part.PartDefinition.Name, "From", from =>
                part.From = XmlConvert.ToDateTime(from, XmlDateTimeSerializationMode.Utc)
            );
            context.ImportAttribute(part.PartDefinition.Name, "Rate", rate =>
                part.Rate = Convert.ToDecimal(rate)
            );
            context.ImportAttribute(part.PartDefinition.Name, "Title", title =>
                part.Title = title
            );
            context.ImportAttribute(part.PartDefinition.Name, "To", to =>
                part.To = XmlConvert.ToDateTime(to, XmlDateTimeSerializationMode.Utc)
            );

        }

        // EXPORT
        protected override void Exporting(SeasonPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("From", XmlConvert.ToString(part.From, XmlDateTimeSerializationMode.Utc));
            context.Element(part.PartDefinition.Name).SetAttributeValue("Rate", Convert.ToString(part.Rate));
            context.Element(part.PartDefinition.Name).SetAttributeValue("Title", part.Title);
            context.Element(part.PartDefinition.Name).SetAttributeValue("To", XmlConvert.ToString(part.To, XmlDateTimeSerializationMode.Utc));

        }

    }

}
