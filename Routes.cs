using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Cascade.Booking
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Guest/Add",
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"},
                            {"controller", "Guest"},
                            {"action", "Add"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Guest/Delete",
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"},
                            {"controller", "Guest"},
                            {"action", "Delete"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Guest/Edit",
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"},
                            {"controller", "Guest"},
                            {"action", "Edit"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Booking/Add",
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"},
                            {"controller", "Booking"},
                            {"action", "Add"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Booking/Delete/{id}",
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"},
                            {"controller", "Booking"},
                            {"action", "Delete"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Booking/Edit/{id}",
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"},
                            {"controller", "Booking"},
                            {"action", "Edit"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Booking/Update",
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"},
                            {"controller", "Booking"},
                            {"action", "Update"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Booking/Bookings",
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"},
                            {"controller", "Booking"},
                            {"action", "Bookings"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/Booking",
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"},
                            {"controller", "Admin"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "Admin/Season",
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"},
                            {"controller", "AdminSeason"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Cascade.Booking"}
                        },
                        new MvcRouteHandler())
                }

                //,
                // new RouteDescriptor {
                //    Priority = 5,
                //    Route = new Route(
                //        "Show",
                //        new RouteValueDictionary {
                //            {"area", "Cascade.PhotoSwipe"},
                //            {"controller", "Show"},
                //            {"action", "Index"}
                //        },
                //        new RouteValueDictionary(),
                //        new RouteValueDictionary {
                //            {"area", "Cascade.PhotoSwipe"}
                //        },
                //        new MvcRouteHandler())
                //}
            };
        }
    }
}