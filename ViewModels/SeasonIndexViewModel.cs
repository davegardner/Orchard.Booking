using System.Collections.Generic;

namespace Cascade.Booking.ViewModels
{
    public class SeasonIndexViewModel
    {
        public SeasonIndexViewModel() {
            Seasons = new List<SeasonSummaryViewModel>();
        }
        public SeasonIndexOptions Options { get; set; }
        public IList<SeasonSummaryViewModel> Seasons { get; set; }
        //public IEnumerable<string> Categories { get; set; }
        public dynamic Pager { get; set; }
    }
}
