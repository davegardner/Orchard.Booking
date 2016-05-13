using Cascade.Booking.Models;
using Cascade.Booking.Services;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cascade.Booking.Controllers
{
    public class BookingController : Controller
    {
        private readonly IContentManager cm;
        private readonly IBookingService bs;
        public BookingController(IContentManager contentManager, IBookingService bookingService)
        {
            cm = contentManager;
            bs = bookingService;
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
            //var json = new
            //{
            //    name = booking.Name,
            //    year = booking.Year,
            //    guests = (from guest in booking.Guests
            //        select new
            //        {
            //            deleted = guest.Deleted,
            //            lastName = guest.LastName,
            //            firstName = guest.FirstName,
            //            category = guest.Category,
            //            fromo = guest.From,
            //            to = guest.To,
            //            costPerNight = guest.CostPerNight,
            //            totalCost = guest.TotalCost
            //        }).ToArray()
            //};

            return Json(booking, JsonRequestBehavior.AllowGet);
        }
    }
}