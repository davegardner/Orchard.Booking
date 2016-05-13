using Orchard.UI.Resources;

namespace Cascade.Poll {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineStyle("BookingAdmin").SetUrl("/Modules/Orchard.jQuery/Styles/jquery-ui.css");
            manifest.DefineStyle("Booking").SetUrl("booking.css");
            manifest.DefineScript("CascadeBooking").SetUrl("cascadebooking.js").SetDependencies("jQuery");
        }
    }
}
