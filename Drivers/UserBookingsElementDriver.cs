using Cascade.Booking.Services;
using Cascade.Booking.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Drivers;
using Orchard.Layouts.Helpers;
using System;

namespace Cascade.Booking.Drivers
{
    public class UserBookingsElementDriver : ElementDriver<Elements.UserBookings>
    {
        private readonly IContentManager cm;
        private readonly IBookingService bs;
        private readonly IWorkContextAccessor wca;

        public UserBookingsElementDriver(
            IContentManager contentManager,
            IWorkContextAccessor workContextAccessor,
            IBookingService bookingService)
        {
            cm = contentManager;
            wca = workContextAccessor;
            bs = bookingService;
        }

        protected override EditorResult OnBuildEditor(Elements.UserBookings element, ElementEditorContext context)
        {

            var viewModel = new BookingDetailsViewModel();
            var editor = context.ShapeFactory.EditorTemplate(TemplateName: "Elements.UserBookings", Model: viewModel);

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

        protected override void OnDisplaying(Elements.UserBookings element, ElementDisplayingContext context)
        {
            // Display a list of bookings that the user has made and allow them to choose which one they want to edit
            // Display the selected booking details
            // Provide a url for them to post changes to

            // Also allow to create a new booking

            var user = wca.GetContext().CurrentUser;

            var bookings = bs.GetBookings(user);

            context.ElementShape.Bookings = bookings;
        }


        //----------------------------- IMPORT / EXPORT ------------------------------------

        protected override void OnExporting(Elements.UserBookings element, ExportElementContext context)
        {


            //if (booking == null)
            //    return;

            //context.ExportableData["UserBookingsElement"] = cm.GetItemMetadata(booking).Identity.ToString();
        }

        protected override void OnImporting(Elements.UserBookings element, ImportElementContext context)
        {
            //var bookingIdentity = context.ExportableData.Get("UserBookingsElement");
            //var booking = !String.IsNullOrWhiteSpace(bookingIdentity) ? context.Session.GetItemFromSession(bookingIdentity) : default(Orchard.ContentManagement.ContentItem);

            //element.Id = booking.Id;
        }

    }
}