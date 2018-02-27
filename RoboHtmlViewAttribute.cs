namespace RoboUI
{
    public class RoboHtmlViewAttribute : RoboControlAttribute
    {
        public RoboHtmlViewAttribute(string viewName)
        {
            ViewName = viewName;
        }

        public string ViewName { get; set; }

        public object Model { get; set; }

        public override bool HasLabelControl => false;
    }
}
