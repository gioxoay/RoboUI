using System;
using System.Collections.Generic;
using System.Reflection;

namespace RoboUI
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RoboControlAttribute : Attribute
    {
        private IDictionary<string, object> htmlAttributes;
        private bool hasLabelControl;

        public RoboControlAttribute()
        {
            hasLabelControl = true;
            ContainerRowIndex = -100;
            ContainerCssClass = "col-sm-12";
        }

        public int ColumnWidth { get; set; }

        public string ContainerCssClass { get; set; }

        public int ContainerColumnIndex { get; set; }

        public int ContainerRowIndex { get; set; }

        public IDictionary<string, object> ContainerHtmlAttributes { get; set; }

        public string ControlContainerCssClass { get; set; }

        public string ControlSpan { get; set; }

        public virtual bool HasLabelControl
        {
            get => hasLabelControl;
            set => hasLabelControl = value;
        }

        public string HelpText { get; set; }

        public virtual bool HideLabelControl { get; set; }

        public IDictionary<string, object> HtmlAttributes
        {
            get => htmlAttributes ?? (htmlAttributes = new Dictionary<string, object>());
            set => htmlAttributes = value;
        }

        public bool IsReadOnly { get; set; }

        public bool IsRequired { get; set; }

        public string LabelCssClass { get; set; }

        public string LabelText { get; set; }

        public string Name { get; set; }

        public short Order { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public string PropertyName { get; set; }

        public Type PropertyType { get; set; }

        public object Value { get; set; }

        #region Implementation of ICloneable<RoboUIHtmlControlAttribute>

        public RoboControlAttribute DeepCopy()
        {
            return ShallowCopy();
        }

        public RoboControlAttribute ShallowCopy()
        {
            return (RoboControlAttribute)MemberwiseClone();
        }

        #endregion Implementation of ICloneable<RoboUIHtmlControlAttribute>
    }
}
