using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
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
            CostPerNight = guest.CostPerNight;
        }

        public int Id { get; set; }
        public int Sequence { get; set; }    // not persisted
        public bool Deleted { get; set; }   // not persisted
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public GuestCategory Category { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public Decimal CostPerNight { get; set; }
        public Decimal? TotalCost
        {
            get { return CostPerNight * NumberOfNights; }
        }

        public int? NumberOfNights
        {
            get
            {
                int? nights = 0;
                try
                {
                    nights = Convert.ToInt32(Math.Round((DateTime.Parse(To) - DateTime.Parse(From)).TotalDays, 0));
                }
                catch
                {
                    nights = null;
                }
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
                        guest.From + FieldBreak +
                        guest.To +
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
                    Guest guest = new Guest
                    {
                        Deleted = false,
                        Sequence = 0,
                        Id = Convert.ToInt32(fields[0]),
                        LastName = fields[1],
                        FirstName = fields[2],
                        Category = (GuestCategory)Enum.Parse(typeof(GuestCategory), fields[3]),
                        CostPerNight = Convert.ToDecimal(fields[4]),
                        From = fields[5],
                        To = fields[6]
                    };
                    guests.Add(guest);
                }
            }
            return guests;
        }

    }

}