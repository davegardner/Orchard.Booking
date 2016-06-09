using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Cascade.Booking.Models
{
    public class Guest
    {
        private const char FieldBreak = '\t';
        private const char RecordBreak = '\r';

        public Guest()
        {

        }

        public Guest(Guest guest)
        {
            Id = 0;
            Sequence = 0;
            Deleted = false;
            LastName = guest.LastName;
            FirstName = guest.FirstName;
            Category = guest.Category;
            From = guest.From;
            To = guest.To;
            Days = guest.Days;
            CostPerNight = guest.CostPerNight;
        }

        public int Id { get; set; }
        public int Sequence { get; set; }    // not persisted
        public bool Deleted { get; set; }   // not persisted
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public GuestCategory Category { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public IEnumerable<Day> Days { get; set; }
        public Decimal CostPerNight { get; set; }
        public Decimal? TotalCost
        {
            get { return Days.Sum(d => d.Cost); }
        }

        public int? NumberOfNights
        {
            get
            {
                int? nights = null;

                if (!To.HasValue || !From.HasValue)
                    return nights;

                var from = From.Value;
                // To times are up to the last second in the day (23:59:59)
                // From times are from the first second in the day
                // so compensate...
                var to = To.Value.AddSeconds(1);
                var duration = (to - from);
                nights = duration.Days;
                return nights;
            }
        }


        public static string Serialize(IEnumerable<Guest> guests)
        {
            string raw = String.Empty;
            if (guests != null && guests.Count() > 0)
            {
                foreach (var guest in guests)
                {
                    raw += guest.Id.ToString() + FieldBreak +
                        guest.LastName + FieldBreak +
                        guest.FirstName + FieldBreak +
                        guest.Category + FieldBreak +
                        guest.CostPerNight.ToString() + FieldBreak +
                        (guest.From.HasValue ? guest.From.Value.ToString("o") : "") + FieldBreak +
                        (guest.To.HasValue ? guest.To.Value.ToString("o") : "") + FieldBreak +
                        Base64Encode(Day.Serialize(guest.Days)) +
                        RecordBreak;
                }
            }
            return raw;
        }

        public static IList<Guest> Deserialize(string raw)
        {
            var guests = new List<Guest>();
            if (!String.IsNullOrWhiteSpace(raw))
            {
                var lines = raw.Split(new[] { RecordBreak }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {

                    var fields = line.Split(FieldBreak);

                    // The following is only needed because days were added to existing data so need to
                    // take care of missing field case and avoid null exceptions
                    var decodedDays = String.Empty;
                    if (fields.Length > 7)
                        decodedDays = fields[7];

                    DateTime from = new DateTime();
                    DateTime.TryParseExact(fields[5], "o", CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind, out from);
                    DateTime to = new DateTime();
                    DateTime.TryParseExact(fields[6], "o", CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind, out to);

                    Guest guest = new Guest
                    {
                        Deleted = false,
                        Sequence = 0,
                        Id = Convert.ToInt32(fields[0]),
                        LastName = fields[1],
                        FirstName = fields[2],
                        Category = (GuestCategory)Enum.Parse(typeof(GuestCategory), fields[3]),
                        CostPerNight = Convert.ToDecimal(fields[4]),
                        From = from,
                        To = to,
                        Days = Day.Deserialize(Base64Decode(decodedDays))
                    };
                    guests.Add(guest);
                }
            }
            return guests;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

}