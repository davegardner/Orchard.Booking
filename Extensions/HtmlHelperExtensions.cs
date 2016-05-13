using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Cascade.Booking.Extensions
{
    //http://blogs.msdn.com/b/stuartleeks/archive/2010/05/21/asp-net-mvc-creating-a-dropdownlist-helper-for-enums.aspx

    public static class HtmlHelperExtensions
    {
        private static readonly SelectListItem[] SingleEmptyItem = new[] { new SelectListItem { Text = "", Value = "" } };

        private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;

            Type underlyingType = Nullable.GetUnderlyingType(realModelType);
            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }
            return realModelType;
        }
        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name, TEnum selectedValue)
        {
            IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>();

            IEnumerable<SelectListItem> items =
                from value in values
                select new SelectListItem
                {
                    Text = value.ToString(),
                    Value = value.ToString(),
                    Selected = (value.Equals(selectedValue))
                };

            return htmlHelper.DropDownList(
                name,
                items
                );
        }
       
    }
}
