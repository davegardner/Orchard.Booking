using Cascade.Booking.Models;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cascade.Booking.Services
{
    public interface IBookingService : Orchard.IDependency
    {
        /// <summary>
        /// Splits guest bookings that extend over more than one region (eg shoulder to high) into two or more guest books that do not overlap
        /// season boundaries. Needed because rates can be different for each part of the season.
        /// </summary>
        /// <returns>normalized list of guests</returns>
        void NormalizeGuestBookings(BookingPart booking);
        void Delete(int id);
        IEnumerable<BookingPart> GetAllBookings(int year = 0);
        BookingPart Get(int id);
        void SetBookingState(int id, BookingState state);
        void SaveOrUpdate(BookingPart bookingPart);
        void DeleteSeason(int id);
        IEnumerable<SeasonPart> GetAllSeasons();
        SeasonPart GetSeason(int id);
        void SaveOrUpdateSeason(SeasonPart seasonPart);
    }

    // Implementation ////////////////////////////////////
    public class BookingService : IBookingService
    {
        private readonly IContentManager cm;
        public BookingService(IContentManager contentManager)
        {
            cm = contentManager;
        }

        public void Delete(int id)
        {
            var poll = Get(id);
            if (poll != null)
                cm.Remove(poll.ContentItem);
        }

        public void DeleteSeason(int id)
        {
            throw new NotImplementedException();
        }

        public BookingPart Get(int id)
        {
            if (id > 0)
            {
                return cm.Get<BookingPart>(id);
            }
            return null;
        }

        public IEnumerable<BookingPart> GetAllBookings(int year = 0)
        {
            IEnumerable<BookingPart> bookings;
            if(year == 0)
                bookings = cm.Query<BookingPart, BookingRecord>().List();
            else
                bookings = cm.Query<BookingPart, BookingRecord>().Where(b => b.Year == year).List();
            return bookings;
        }

        public IEnumerable<SeasonPart>GetAllSeasons()
        {
            return cm.Query<SeasonPart, SeasonRecord>().List();
        }

        public SeasonPart GetSeason(int id)
        {
            if (id == 0)
                return null;

            return cm.Get<SeasonPart>(id);
        }

        public void NormalizeGuestBookings(BookingPart booking)
        {
            //    var guests = booking.Guests.ToList();
            //    var season = booking.SeasonPart;

            //    foreach (var guest in guests)
            //    {
            //        var numberOfGuests = guests.Count;
            //        var index = guests.IndexOf(guest);
            //        if (guest.From.Year != booking.SeasonPart.Year || guest.To.Year != booking.SeasonPart.Year)
            //            throw new Exception("Attempted to book a guest for a period outside the current Season");

            //        // if a guest's booking dates extend over more than one season segment then create a new 'guest' for each
            //        // segment

            //        if (guest.From < season.ShoulderFrom && guest.To > season.ShoulderFrom)
            //        {
            //            guests.Insert(index++, new Guest
            //            {
            //                From = guest.From,
            //                To = season.ShoulderFrom < guest.To ? season.ShoulderFrom : guest.To
            //            });
            //            guest.From = season.ShoulderFrom;
            //        }

            //        if (guest.From < season.PeakFrom && guest.To > season.PeakFrom)
            //        {
            //            guests.Insert(index++, new Guest
            //            {
            //                From = guest.From,
            //                To = season.PeakFrom < guest.To ? season.PeakFrom : guest.To
            //            });
            //            guest.From = season.PeakFrom;
            //        }

            //        if (guest.From < season.PeakTo && guest.To > season.PeakTo)
            //        {
            //            guests.Insert(index++, new Guest
            //            {
            //                From = guest.From,
            //                To = season.PeakTo < guest.To ? season.PeakTo : guest.To
            //            });
            //            guest.From = season.PeakTo;
            //        }

            //        if (guest.From < season.ShoulderTo && guest.To > season.ShoulderTo)
            //        {
            //            guests.Insert(index++, new Guest
            //            {
            //                From = guest.From,
            //                To = season.ShoulderTo < guest.To ? season.ShoulderTo : guest.To
            //            });
            //            guest.From = season.ShoulderTo;
            //        }

            //        if (guest.From > season.ShoulderTo)
            //        {
            //            guests.Insert(index++, new Guest
            //            {
            //                From = guest.From,
            //                To = guest.To
            //            });
            //        }

            //        // if new segments were created then delete the original guest
            //        if (numberOfGuests < guests.Count)
            //        {
            //            guests.Remove(guest);
            //        }
            //    }

            //    booking.Guests = guests;
        }

        public void SaveOrUpdate(BookingPart bookingPart)
        {
            cm.Publish(bookingPart.ContentItem);
        }

        public void SaveOrUpdateSeason(SeasonPart seasonPart)
        {
            cm.Publish(seasonPart.ContentItem);
        }

        public void SetBookingState(int id, BookingState state)
        {
            Get(id).BookingState = state;
        }
    }
}