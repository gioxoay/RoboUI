using System;

namespace RoboUI
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RoboHtmlAttributeAttribute : Attribute
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public bool IsContainer { get; set; }

        public RoboHtmlAttributeAttribute(string name, string value = null)
        {
            Name = name;
            Value = value;
        }
    }
}
