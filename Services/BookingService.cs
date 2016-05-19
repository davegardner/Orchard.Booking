using Cascade.Booking.Models;
using Orchard.ContentManagement;
using Orchard.Localization.Services;
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
        void SaveOrUpdateBooking(BookingPart bookingPart);
        void DeleteSeason(int id);
        IEnumerable<SeasonPart> GetAllSeasons();
        SeasonPart GetSeason(int id);
        SeasonPart GetSeason(Guest guest);
        void SaveOrUpdateSeason(SeasonPart seasonPart);
    }

    // Implementation ////////////////////////////////////
    public class BookingService : IBookingService
    {
        private readonly IDateLocalizationServices dls;
        private readonly IContentManager cm;
        public BookingService(IContentManager contentManager, IDateLocalizationServices dateLocationServices)
        {
            cm = contentManager;
            dls = dateLocationServices;
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
            bookings = cm.Query<BookingPart, BookingRecord>().List();
            return bookings;
        }

        public IEnumerable<SeasonPart> GetAllSeasons()
        {
            return cm.Query<SeasonPart, SeasonRecord>()
                .OrderByDescending(s=>s.FromDate)
                .List();
        }

        public SeasonPart GetSeason(int id)
        {
            if (id == 0)
                return null;

            return cm.Get<SeasonPart>(id);
        }

        public SeasonPart GetSeason(Guest guest)
        {
            if (guest == null)
                return null;
            var seasons = GetAllSeasons();
            var from = guest.From;
            return seasons.FirstOrDefault(s => s.From <= from && from  < s.To);
        }

        public void NormalizeGuestBookings(BookingPart booking)
        {
            var guests = booking.Guests.ToList();
            var seasons = GetAllSeasons().ToList();
            foreach (var guest in guests)
            {
                Normalize(seasons, booking, guest);
            }
        }

        public void SaveOrUpdateBooking(BookingPart bookingPart)
        {
            NormalizeGuestBookings(bookingPart);
            cm.Publish(bookingPart.ContentItem);
        }

        public void SaveOrUpdateSeason(SeasonPart seasonPart)
        {
            cm.Publish(seasonPart.ContentItem);
        }

        public void SetBookingState(int id, BookingState state)
        {
            var booking = Get(id);
            booking.BookingState = state;
            SaveOrUpdateBooking(booking);
        }

        /// <summary>
        /// The season determines the guest Cost Per Night. The person booking may enter from/to dates
        /// that cover more than one season. To cater for this we split the Guest into one guest record 
        /// per season, so there are no overlaps.
        /// </summary>
        private void Normalize(IEnumerable<SeasonPart> seasons, BookingPart booking, Guest currentGuest)
        {
            // make a list of all except the current guest. Current guest gets added back later.
            var guestList = booking.Guests
                .Except(new List<Guest> { currentGuest })
                .ToList();

            // Do date conversions just once
            DateTime? guestFrom = currentGuest.From;
            DateTime? guestTo = currentGuest.To;

            // Get a list of seasons that are partly or wholly covered by this guest booking
            var coveredSeasons = seasons
                .Where(s => guestFrom  <= s.From && s.To <= guestTo       // totally enclosed
                    || s.From <= guestFrom && guestFrom <= s.To         // start season
                    || s.From <= guestTo && guestTo <= s.To)            // end season
                .OrderBy(s => s.From);

            // create one guest record per covered season
            guestList.AddRange(coveredSeasons.Select(s => new Guest
            {
                Id = 0,
                Sequence = 0,
                Deleted = false,
                LastName = currentGuest.LastName,
                FirstName = currentGuest.FirstName,
                Category = currentGuest.Category,
                From = MaxDate(guestFrom, s.From),
                To = MinDate(guestTo, s.To),
                CostPerNight = s.Rate
            }));

            // replace guest list
            booking.Guests = guestList;
        }

        private DateTime? MaxDate(DateTime? d1, DateTime? d2)
        {
            if (d1 > d2)
                return d1;
            return d2;
        }
        private DateTime? MinDate(DateTime? d1, DateTime? d2)
        {
            if (d1 < d2)
                return d1;
            return d2;
        }

    }
}