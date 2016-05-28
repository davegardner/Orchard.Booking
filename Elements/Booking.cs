using Orchard.Layouts.Framework.Elements;
using Orchard.Layouts.Helpers;

namespace Cascade.Booking.Elements
{
    // Render the booking element form on the frontend

    public class Booking: Element
    {
        public override string Category
        {
            get { return "Forms"; }
        }

        public override string ToolboxIcon
        {
            get { return "\uf274"; }
        }

        public int Id
        {
            get { return this.Retrieve(x => x.Id); }
            set { this.Store(x => x.Id, value); }
        }

        public int Name
        {
            get { return this.Retrieve(x => x.Name); }
            set { this.Store(x => x.Name, value); }
        }
    }
}