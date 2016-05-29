using Orchard.UI.Resources;

namespace Cascade.Poll {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineStyle("Booking").SetUrl("Booking.min.css", "Booking.css");
            manifest.DefineStyle("BookingAdmin").SetUrl("BookingAdmin.min.css", "BookingAdmin.css");
            manifest.DefineScript("CascadeBooking").SetUrl("cascadebooking.js").SetDependencies("jQuery");
        }
    }
}
