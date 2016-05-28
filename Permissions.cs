using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Cascade.Booking {
    public class Permissions : IPermissionProvider {
        public static readonly Permission AddBooking = new Permission { Description = "Add booking", Name = "AddBooking" };
        public static readonly Permission ManageBookings = new Permission { Description = "Manage bookings", Name = "ManageBookings" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                AddBooking,
                ManageBookings,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManageBookings, AddBooking}
                },
                new PermissionStereotype {
                    Name = "Authenticated",
                    Permissions = new[] {AddBooking}
                },
                new PermissionStereotype {
                    Name = "Editor",
                    Permissions = new[] {AddBooking}
                },
                new PermissionStereotype {
                    Name = "Moderator",
                    Permissions = new[] {ManageBookings, AddBooking}
                },
                new PermissionStereotype {
                    Name = "Author",
                    Permissions = new[] {AddBooking}
                },
                new PermissionStereotype {
                    Name = "Contributor",
                    Permissions = new[] {AddBooking}
                },
            };
        }
    }
}
