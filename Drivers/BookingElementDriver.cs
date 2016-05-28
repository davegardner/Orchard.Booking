using Cascade.Booking.Services;
using Cascade.Booking.ViewModels;
using Orchard.ContentManagement;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Drivers;
using Orchard.Layouts.Helpers;
using System;

namespace Cascade.Booking.Drivers
{
    public class BookingElementDriver : ElementDriver<Elements.Booking>
    {
        private readonly IContentManager cm;
        private readonly IBookingService bs;

        public BookingElementDriver(
            IContentManager contentManager,
            IBookingService bookingService)
        {
            cm = contentManager;
            bs = bookingService;
        }

        protected override EditorResult OnBuildEditor(Elements.Booking element, ElementEditorContext context)
        {

            var viewModel = new BookingDetailsViewModel();
            var editor = context.ShapeFactory.EditorTemplate(TemplateName: "Elements.Booking", Model: viewModel);

            //if (context.Updater != null)
            //{
            //    context.Updater.TryUpdateModel(viewModel, context.Prefix, null, null);
            //    element.MediaId = ParseImageId(viewModel.ImageId);
            //    element.Width = viewModel.Width;
            //    element.Height = viewModel.Height;
            //    element.Etc = viewModel.Etc;
            //    element.Responsive = viewModel.Responsive;
            //}

            //var imageId = element.MediaId;
            //if (imageId == null)
            //{
            //    viewModel.CurrentImage = default(ImagePart);
            //    viewModel.Responsive = true;
            //}
            //else
            //{
            //    viewModel.CurrentImage = GetImage(imageId.Value);
            //}
            //viewModel.Width = element.Width;
            //viewModel.Height = element.Height;
            //viewModel.Etc = element.Etc;
            //viewModel.Responsive = element.Responsive;

            return Editor(context, editor);
        }

        protected override void OnDisplaying(Elements.Booking element, ElementDisplayingContext context)
        {
            // Display a list of bookings that the user has made and allow them to choose which one they want to edit
            // Display the selected booking details
            // Provide a url for them to post changes to

            // Also allow to create a new booking


            var id = element.Id;
            var booking = bs.Get(27);
            context.ElementShape.Booking = booking;
        }


        //----------------------------- IMPORT / EXPORT ------------------------------------

        protected override void OnExporting(Elements.Booking element, ExportElementContext context)
        {
            var booking = bs.Get(element.Id);

            if (booking == null)
                return;

            context.ExportableData["BookingElement"] = cm.GetItemMetadata(booking).Identity.ToString();
        }

        protected override void OnImporting(Elements.Booking element, ImportElementContext context)
        {
            var bookingIdentity = context.ExportableData.Get("BookingElement");
            var booking = !String.IsNullOrWhiteSpace(bookingIdentity) ? context.Session.GetItemFromSession(bookingIdentity) : default(Orchard.ContentManagement.ContentItem);

            element.Id = booking.Id;
        }

    }
}