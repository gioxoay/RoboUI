namespace RoboUI
{
    public class RoboTimePickerAttribute : RoboControlAttribute
    {
        public RoboTimePickerAttribute()
        {
            Interval = 30;
            EnableTypingText = true;
        }

        public int Interval { get; set; }

        public bool EnableTypingText { get; set; }
    }
}
