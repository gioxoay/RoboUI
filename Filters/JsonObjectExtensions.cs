using System.Collections.Generic;
using System.Linq;

namespace RoboUI.Filters
{
    public static class JsonObjectExtensions
    {
        public static IEnumerable<IDictionary<string, object>> ToJson(IEnumerable<JsonObject> items)
        {
            return items.Select(i => i.ToJson());
        }
    }
}
