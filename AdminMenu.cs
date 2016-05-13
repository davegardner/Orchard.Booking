using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Core.Contents;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Cascade.Booking
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.AddImageSet("booking")
                .Add(T("Bookings"), "6", menu => menu
                    .Add(T("List"), "0", item => item.Action("Index", "Admin", new { area = "Cascade.Booking" }))
                    .Add(T("Seasons"), "1", item => item.Action("Index", "AdminSeason", new { area = "Cascade.Booking" }))
                 );
        }
    }
}
