using System.Dynamic;
using Newtonsoft.Json;

namespace RoboUI.Extensions
{
    internal static class ObjectExtensions
    {
        public static T Clone<T>(this T item) where T : class
        {
            if (item == null)
            {
                return null;
            }

            var json = JsonConvert.SerializeObject(item);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            dynamic d = anonymousObject;
            return d;
        }
    }
}
