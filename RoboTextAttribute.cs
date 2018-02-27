namespace RoboUI
{
    public class RoboTextAttribute : RoboControlAttribute
    {
        public RoboTextAttribute()
        {
            Type = RoboTextType.TextBox;
        }

        public RoboTextAttribute(RoboTextType type)
        {
            Type = type;
        }

        public RoboTextType Type { get; set; }

        public int MinLength { get; set; }

        public int MaxLength { get; set; }

        public string EqualTo { get; set; }

        public string MessageValidate { get; set; }

        public int Cols { get; set; }

        public int Rows { get; set; }

        public string RegexPattern { get; set; }

        public string RegexValue { get; set; }
    }

    public enum RoboTextType : byte
    {
        TextBox,
        Password,
        Email,
        Url,
        MultiText,
        RichText
    }
}
