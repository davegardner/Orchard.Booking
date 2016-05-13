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

        public int Id { get; set; }
        public int Sequence{ get; set; }    // not persisted
        public bool Deleted { get; set; }   // not persisted
        public string LastName{ get; set; }
        public string FirstName { get; set; }
        public GuestCategory Category { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Decimal CostPerNight { get; set; }
        public Decimal TotalCost {
            get{ return CostPerNight * NumberOfNights; }
        }

        public int NumberOfNights
        {
            get
            {
                return Convert.ToInt32(Math.Round((To.Date - From.Date).TotalDays, 0));
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
                        guest.From.ToString() + FieldBreak +
                        guest.To.ToString() +
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
                        From = Convert.ToDateTime(fields[5]),
                        To = Convert.ToDateTime(fields[6])
                    };
                    guests.Add(guest);
                }
            }
            return guests;
        }

    }

}