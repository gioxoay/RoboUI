using Microsoft.AspNetCore.Mvc.Rendering;

namespace RoboUI
{
    public class ExtendedSelectListItem : SelectListItem
    {
        public object HtmlAttributes { get; set; }

        public string GroupKey { get; set; }

        public string GroupName { get; set; }
    }
}
