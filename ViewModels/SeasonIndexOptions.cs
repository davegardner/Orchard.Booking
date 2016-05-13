using System.ComponentModel;

namespace Cascade.Booking.ViewModels
{
    public class SeasonIndexOptions 
    {
        public SeasonBulkFilter Filter { get; set; }
        public SeasonBulkAction BulkAction { get; set; }
    }


    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
    public enum SeasonBulkFilter
    {
        All,
        Closed,
        Open
    }
    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
    public enum SeasonBulkAction
    {
        None,
        Delete
    }
}
