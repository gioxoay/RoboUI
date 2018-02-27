using RoboUI.Filters;

namespace RoboUI.Expressions
{
    internal class PropertyToken : IMemberAccessToken
    {
        public PropertyToken(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }
    }
}
