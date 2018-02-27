namespace RoboUI
{
    public class RoboCascadingDropDownAttribute : RoboControlAttribute
    {
        public string ParentControl { get; set; }

        public bool AbsoluteParentControl { get; set; }

        public bool AllowMultiple { get; set; }

        public bool EnableChosen { get; set; }

        public string OnSelectedIndexChanged { get; set; }

        public string OnSuccess { get; set; }
    }

    public class RoboCascadingDropDownOptions
    {
        public string ParentControl { get; set; }

        public string SourceUrl { get; set; }

        public string Command { get; set; }
    }
}
