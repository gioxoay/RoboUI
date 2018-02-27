using System.Collections.Generic;
using System.Linq;

namespace RoboUI
{
    public class RoboUIGridAjaxData<TModel> : List<TModel>
    {
        public RoboUIGridAjaxData()
        {
            UserData = new Dictionary<string, object>();
            Callbacks = new List<string>();
        }

        public RoboUIGridAjaxData(IEnumerable<TModel> items)
            : this()
        {
            AddRange(items);
        }

        public RoboUIGridAjaxData(IEnumerable<TModel> items, int totalRecords)
            : this()
        {
            AddRange(items);
            TotalRecords = totalRecords;
        }

        public int TotalRecords { get; private set; }

        public IDictionary<string, object> UserData { get; set; }

        public ICollection<string> Callbacks { get; private set; }

        public static implicit operator RoboUIGridAjaxData<object>(RoboUIGridAjaxData<TModel> model)
        {
            var result = new RoboUIGridAjaxData<object>();
            result.AddRange(model.Cast<object>());
            result.TotalRecords = model.TotalRecords;
            result.UserData = model.UserData;
            return result;
        }
    }
}
