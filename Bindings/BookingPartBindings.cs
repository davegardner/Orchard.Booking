using Cascade.Booking.Models;
using Orchard;
using Orchard.DynamicForms.Services;
using Orchard.DynamicForms.Services.Models;
using System;

namespace Cascade.Booking.Bindings
{
    // These bindings were intended to support Dynamic Forms. But DF is useless at this time because it doesn't suppport editing existing data.
    // I have left the code here as a reminder of how to do it. Maybe one day DF will bind to existing data.
    public class BookingPartBindings : Component, IBindingProvider
    {
        public void Describe(BindingDescribeContext context)
        {
            context.For<BookingPart>()
                .Binding("Name", (contentItem, part, s) => { part.Name = s; })
                .Binding("BookingState", (contentItem, part, s) => { part.BookingState = (BookingState)Enum.Parse(typeof(BookingState), s); })
                ;
        }
    }
}