using System.Collections.Generic;
using RoboUI.Filters;

namespace RoboUI
{
    public class RoboUIGridRequest
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public IList<SortDescriptor> Sorts { get; set; }

        public IList<IFilterDescriptor> Filters { get; set; }
    }
}