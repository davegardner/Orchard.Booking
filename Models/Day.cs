using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Cascade.Booking.Models
{
    public class Day
    {
        private const char FieldBreak = '\t';
        private const char RecordBreak = '\r';

        public Day()
        {

        }

        public Day(Day day)
        {
            Id = 0;
            Coupon = day.Coupon;
            Date = day.Date;
            Cost = day.Cost;
        }

        public int Id { get; set; }
        public bool Coupon{ get; set; }  
        public DateTime? Date{ get; set; }
        public Decimal Cost { get; set; }
   
       

        public static string Serialize(IEnumerable<Day> days)
        {
            string raw = String.Empty;
            if (days != null && days.Count() > 0)
            {
                foreach (var day in days)
                {
                    raw += day.Id.ToString() + FieldBreak +
                        day.Coupon.ToString() + FieldBreak +
                        (day.Date.HasValue ? day.Date.Value.ToString("o") : "") + FieldBreak +
                        day.Cost.ToString() + 
                        RecordBreak;
                }
            }
            return raw;
        }

        public static IList<Day> Deserialize(string raw)
        {
            var days = new List<Day>();
            if (!String.IsNullOrWhiteSpace(raw))
            {
                var lines = raw.Split(new[] { RecordBreak }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var fields = line.Split(FieldBreak);
                    Day day = new Day
                    {
                        Id = Convert.ToInt32(fields[0]),
                        Coupon = Convert.ToBoolean(fields[1]),
                        Date = DateTime.Parse(fields[2], null, DateTimeStyles.RoundtripKind),
                        Cost = Convert.ToDecimal(fields[3]),
                    };
                    days.Add(day);
                }
            }
            return days;
        }

    }

}