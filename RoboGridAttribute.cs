using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RoboUI
{
    public class RoboGridAttribute : RoboControlAttribute
    {
        private readonly byte defaultRows;
        private List<RoboControlAttribute> attributes;

        public RoboGridAttribute()
        {
            ShowTableHead = true;
            EnabledScroll = false;
        }

        public RoboGridAttribute(byte minRows, byte maxRows)
            : this()
        {
            ShowRowsControl = true;
            defaultRows = minRows;
            MinRows = minRows;
            MaxRows = maxRows;
        }

        public RoboGridAttribute(byte minRows, byte maxRows, byte defaultRows)
            : this()
        {
            if (maxRows < minRows || maxRows == 0)
            {
                throw new ArgumentOutOfRangeException("maxRows");
            }

            if (defaultRows < minRows || defaultRows > maxRows)
            {
                throw new ArgumentOutOfRangeException("defaultRows");
            }

            ShowRowsControl = true;
            this.defaultRows = defaultRows;
            MinRows = minRows;
            MaxRows = maxRows;
        }

        public byte DefaultRows => defaultRows;

        public ICollection<RoboControlAttribute> Attributes => attributes;

        public bool EnabledScroll { get; set; }

        public override bool HasLabelControl => ShowLabelControl;

        public byte MaxRows { get; set; }

        public byte MinRows { get; set; }

        public bool ShowAsStack { get; set; }

        public bool ShowLabelControl { get; set; }

        public bool ShowRowsControl { get; set; }

        public bool ShowTableHead { get; set; }

        public string TableHeadHtml { get; set; }

        public void EnsureProperties()
        {
            if (attributes == null)
            {
                if (!typeof(IEnumerable).IsAssignableFrom(PropertyType) || !PropertyType.GetTypeInfo().IsGenericType)
                {
                    throw new NotSupportedException("Cannot apply robo grid for non enumerable property as grid.");
                }

                var type = PropertyType.GetGenericArguments()[0];
                attributes = new List<RoboControlAttribute>();

                foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var attribute = propertyInfo.GetCustomAttribute<RoboControlAttribute>(false);
                    if (attribute != null)
                    {
                        attribute.Name = propertyInfo.Name;
                        attribute.PropertyName = propertyInfo.Name;
                        attribute.PropertyType = propertyInfo.PropertyType;
                        if (attribute.LabelText == null)
                        {
                            attribute.LabelText = propertyInfo.Name;
                        }

                        var htmlAttributes = propertyInfo.GetCustomAttributes<RoboHtmlAttributeAttribute>().ToList();
                        if (htmlAttributes.Any())
                        {
                            foreach (var htmlAttribute in htmlAttributes)
                            {
                                attribute.HtmlAttributes.Add(htmlAttribute.Name, htmlAttribute.Value);
                            }
                        }

                        attributes.Add(attribute);
                    }
                }

                attributes.Sort((x, y) => x.Order.CompareTo(y.Order));
            }
        }

        public static object GetDefaultValue(Type type)
        {
            if (type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}
